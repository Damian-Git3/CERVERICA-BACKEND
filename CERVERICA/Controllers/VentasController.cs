using CERVERICA.Data;
using CERVERICA.Dtos;
using CERVERICA.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace CERVERICA.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VentasController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public VentasController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("stocks")]
        public async Task<IActionResult> GetStocks()
        {
            var stocks = await _context.Stocks.Where(p => p.Cantidad > 0).Include(s => s.Receta).ToListAsync();

            //var groupedStocks = stocks.GroupBy(s => s.Receta)
            //    .Select(gr => new
            //    {
            //        RecetaId = gr.Key.Id,
            //        Nombre = gr.Key.Nombre,
            //        Imagen = gr.Key.Imagen,
            //        PrecioLitro = gr.Key.PrecioLitro,
            //        Activo = gr.Key.Activo,
            //        StocksPorEnvase = gr.GroupBy(s => new { s.TipoEnvase, s.MedidaEnvase })
            //                            .Select(g => new
            //                            {
            //                                TipoEnvase = g.Key.TipoEnvase,
            //                                MedidaEnvase = g.Key.MedidaEnvase,
            //                                CantidadTotal = g.Sum(s => s.Cantidad),
            //                                PrecioPorEnvase = g.Key.MedidaEnvase * (gr.Key.PrecioLitro / 1000)
            //                            })
            //    })
            //    .ToList();

            return Ok();
        }


        [HttpPost("CrearVenta")]
        public async Task<IActionResult> CrearVenta([FromBody] CrearVentaDto dto)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var venta = new Venta
                {
                    IdUsuario = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "a0a0aaa0-0000-0aa0-0000-00aaa0aa0a00",
                    FechaVenta = DateTime.Now,
                    Total = 0,
                    TipoVenta = dto.TipoVenta
                };

                _context.Ventas.Add(venta);
                await _context.SaveChangesAsync();

                float totalVenta = 0;
                var detallesVentas = new List<DetalleVenta>();

                foreach (var detalle in dto.Detalles.GroupBy(d => d.IdReceta))
                {
                    var recetaId = detalle.Key;
                    var cantidadTotalRequerida = detalle.Sum(d => (d.Pack*0.355) * d.Cantidad);
                    var stocks = await _context.Stocks
                        .Where(s => s.IdReceta == recetaId && s.Cantidad > 0)
                        .OrderBy(s => s.FechaEntrada)
                        .ToListAsync();

                    var cantidadTotalDisponible = stocks.Sum(s => s.Cantidad);
                    if (cantidadTotalDisponible < cantidadTotalRequerida)
                    {
                        var nombreReceta = await _context.Recetas.Where(r => r.Id == recetaId).Select(r => r.Nombre).FirstOrDefaultAsync();
                        return BadRequest(new { message = $"No hay suficiente stock para {nombreReceta}. Litros Requeridos: {cantidadTotalRequerida:f2}, Disponibles: {cantidadTotalDisponible}"});
                    }

                    foreach (var detalleItem in detalle)
                    {
                        while (detalleItem.Cantidad > 0)
                        {
                            var stock = stocks.FirstOrDefault(s => s.Cantidad >= (detalleItem.Pack * 0.355));
                            if (stock == null)
                            {
                                var nombreReceta = await _context.Recetas.Where(r => r.Id == recetaId).Select(r => r.Nombre).FirstOrDefaultAsync();
                                return BadRequest(new { message = $"No hay suficiente stock de la cerveza {nombreReceta}" });
                            }

                            var reduccionMaximaPacks = (int)(stock.Cantidad / (detalleItem.Pack * 0.355));

                            var cantidadADescontar = Math.Min(reduccionMaximaPacks, (detalleItem.Cantidad));
                            stock.Cantidad -= cantidadADescontar;
                            detalleItem.Cantidad -= cantidadADescontar;

                            float montoVenta = detalleItem.Pack switch
                            {
                                1 => stock.Receta.PrecioPaquete1 ?? 0,
                                6 => stock.Receta.PrecioPaquete6 ?? 0,
                                12 => stock.Receta.PrecioPaquete12 ?? 0,
                                24 => stock.Receta.PrecioPaquete24 ?? 0,
                                _ => 0
                            };

                            if (montoVenta == 0)
                            {
                                return BadRequest(new {message = $"Pack de {detalleItem.Pack} de {stock.Receta.Nombre} no se encuentra disponible para la venta."});
                            }

                            var detalleVenta = new DetalleVenta
                            {
                                IdVenta = venta.Id,
                                IdStock = stock.Id,
                                MontoVenta = montoVenta * cantidadADescontar,
                                Cantidad = cantidadADescontar,
                                Pack = detalleItem.Pack
                            };

                            totalVenta += detalleVenta.MontoVenta;
                            detallesVentas.Add(detalleVenta);
                        }
                    }
                }

                venta.Total = totalVenta;
                _context.Ventas.Update(venta);
                _context.DetallesVenta.AddRange(detallesVentas);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return Ok(venta);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, new {message = $"Error al crear la venta"});
            }
        }

        [HttpPost("RegistrarMerma")]
        public async Task<IActionResult> RegistrarMerma(int stockId, int cantidadMerma)
        {
            var stock = await _context.Stocks.FirstOrDefaultAsync(s => s.Id == stockId);

            if (stock == null)
            {
                return NotFound($"No se encontró el stock con ID {stockId}");
            }

            if (cantidadMerma <= 0)
            {
                return BadRequest("La cantidad de merma debe ser mayor a cero");
            }

            if (cantidadMerma > stock.Cantidad)
            {
                return BadRequest("La cantidad de merma no puede ser mayor a la cantidad disponible en el stock");
            }

            stock.Cantidad -= cantidadMerma;
            stock.Merma = (stock.Merma ?? 0) + cantidadMerma;

            _context.Stocks.Update(stock);
            await _context.SaveChangesAsync();

            return Ok(stock);
        }
    }
}
