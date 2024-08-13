using CERVERICA.Data;
using CERVERICA.Dtos;
using CERVERICA.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Security.Claims;

namespace CERVERICA.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VentasController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public VentasController(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        [HttpGet("cliente/{id}")]
        public async Task<List<VentasClienteDto>> GetVentasCliente(string id)
        {
            return await _context.Ventas.Where(p => p.IdUsuario == id).Select(
                p => new VentasClienteDto
                {
                    FechaVenta = p.FechaVenta,
                    Total = p.Total,
                    MetodoEnvio = p.MetodoEnvio,
                    MetodoPago = p.MetodoPago,
                    EstatusVenta = p.EstatusVenta
                }).ToListAsync();
        }

        //get todo el stock
        [HttpGet("stock")]
        public async Task<List<Stock>> GetStock()
        {
            return await _context.Stocks.ToListAsync();
        }

        [HttpPost("CrearVenta")]
        public async Task<IActionResult> CrearVenta([FromBody] CrearVentaDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(currentUserId!);

            if (user is null)
            {
                return NotFound(new AuthResponseDto
                {
                    IsSuccess = false,
                    Message = "User not found"
                });
            }

            var productosEliminados = new List<string>();

            foreach (var detalleVenta in dto.Detalles)
            {
                var stocks = await _context.Stocks
                    .Where(s => s.IdReceta == detalleVenta.IdReceta && s.Cantidad > 0)
                    .OrderBy(s => s.FechaEntrada)
                    .ToListAsync();

                var cantidadTotalDisponible = stocks.Sum(s => s.Cantidad);

                if (cantidadTotalDisponible < detalleVenta.Pack * detalleVenta.Cantidad)
                {
                    var productosCarritoEliminar = await _context.ProductosCarrito
                        .Where(productoCarrito => productoCarrito.IdUsuario == currentUserId && productoCarrito.IdReceta == detalleVenta.IdReceta && productoCarrito.CantidadLote == detalleVenta.Pack)
                        .ToListAsync();

                    foreach (var productoCarrito in productosCarritoEliminar)
                    {
                        var receta = await _context.Recetas.Where(r => r.Id == detalleVenta.IdReceta).FirstOrDefaultAsync();
                        productosEliminados.Add(receta.Nombre + " - " + productoCarrito.CantidadLote + " Pack");
                        _context.ProductosCarrito.Remove(productoCarrito);
                    }
                }
            }

            if (productosEliminados.Any())
            {
                await _context.SaveChangesAsync();
                return StatusCode(409, new
                {
                    message = "Se eliminaron productos de tu carrito que no tenemos stock",
                    productosCarritoEliminados = productosEliminados
                });
            }

            // Si no hubo problemas de stock, guardar los cambios
            await _context.SaveChangesAsync();

            using var transaction = await _context.Database.BeginTransactionAsync();
            
            try
            {
                var venta = new Venta();

                venta.IdUsuario = user.Id;
                venta.FechaVenta = DateTime.Now;
                venta.EstatusVenta = EstatusVenta.Recibido;
                venta.MetodoEnvio = dto.MetodoEnvio;
                venta.MetodoPago = dto.MetodoPago;

                if (dto.MetodoEnvio == MetodoEnvio.EnvioDomicilio)
                {
                    venta.NombrePersonaRecibe = dto.NombrePersonaRecibe;
                    venta.Calle = dto.Calle;
                    venta.NumeroCasa = dto.NumeroCasa;
                    venta.CodigoPostal = dto.CodigoPostal;
                    venta.Ciudad = dto.Ciudad;
                    venta.Estado = dto.Estado;
                }

                if (dto.MetodoPago == MetodoPago.TarjetaCredito)
                {
                    venta.NombrePersonaTarjeta = dto.NombrePersonaTarjeta;
                    venta.NumeroTarjeta = dto.NumeroTarjeta;
                    venta.MesExpiracion = dto.MesExpiracion;
                    venta.AnioExpiracion = dto.AnioExpiracion;
                    venta.CVV = dto.CVV;
                }

                _context.Ventas.Add(venta);
                await _context.SaveChangesAsync();

                float totalVenta = 0;
                var detallesVentas = new List<DetalleVenta>();

                //calculamos de que stock se van a restar los productos
                //solo se puede descontar de un stock que tenga el mismo tipo de envase y de medida de envase

                //considerar que el stock de una receta contiene productos de diferentes tamañanos y tipo de envase y la cantidad del stock 
                //no es una cantidad en litros sino del total de botellas que se tienen en stock

                foreach (var detalle in dto.Detalles)
                {
                    var recetaId = detalle.IdReceta;
                    var cantidadTotalRequerida = detalle.Pack * detalle.Cantidad;

                    var stocks = await _context.Stocks
                        .Where(s => s.IdReceta == recetaId && s.Cantidad >= detalle.Pack && s.MedidaEnvase == detalle.MedidaEnvase && s.TipoEnvase == detalle.TipoEnvase)
                        .OrderBy(s => s.FechaEntrada)
                        .ToListAsync();

                    var cantidadTotalDisponible = stocks.Sum(s => s.Cantidad);
                    if (cantidadTotalDisponible < cantidadTotalRequerida)
                    {
                        var nombreReceta = await _context.Recetas.Where(r => r.Id == recetaId).Select(r => r.Nombre).FirstOrDefaultAsync();

                        return StatusCode(409, (new { message = $"No hay suficiente stock para {nombreReceta}. Cervezas Requeridas: {cantidadTotalRequerida:f2}, Disponibles: {cantidadTotalDisponible}"}));

                    }

                        while (detalle.Cantidad > 0)
                        {
                            var stock = stocks.FirstOrDefault();
                            if (stock == null)
                            {
                                var nombreReceta = await _context.Recetas.Where(r => r.Id == recetaId).Select(r => r.Nombre).FirstOrDefaultAsync();
                                return BadRequest(new { message = $"No hay suficiente stock de la cerveza {nombreReceta}" });
                            }

                            var reduccionMaximaPacks = (int)(stock.Cantidad / detalle.Pack);

                            var cantidadADescontar = Math.Min(reduccionMaximaPacks, (detalle.Cantidad));
                            stock.Cantidad -= cantidadADescontar*detalle.Pack;
                            detalle.Cantidad -= cantidadADescontar;
                            
                            var receta = await _context.Recetas.FirstOrDefaultAsync(r => r.Id == recetaId);

                            if(receta == null)
                            {
                                return NotFound(new{message= $"No se encontró la receta"});
                            }

                            float montoVenta = detalle.Pack switch
                            {
                                1 => receta.PrecioPaquete1 ?? 0,
                                6 => receta.PrecioPaquete6 ?? 0,
                                12 => receta.PrecioPaquete12 ?? 0,
                                24 => receta.PrecioPaquete24 ?? 0,
                                _ => 0
                            };

                            if (montoVenta == 0)
                            {
                                return BadRequest(new {message = $"Pack de {detalle.Pack} de {receta.Nombre} no se encuentra disponible para la venta."});

                            }
                        
                        var detalleVenta = new DetalleVenta
                            {
                                IdVenta = venta.Id,
                                IdStock = stock.Id,
                                MontoVenta = montoVenta * cantidadADescontar,
                                Cantidad = cantidadADescontar,
                                Pack = detalle.Pack
                            };

                            totalVenta += detalleVenta.MontoVenta;
                            detallesVentas.Add(detalleVenta);
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
                return StatusCode(500, new { message = $"Error al crear la venta " + ex.Message });
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
