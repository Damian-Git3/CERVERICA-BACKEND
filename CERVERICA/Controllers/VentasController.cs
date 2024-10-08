﻿using CERVERICA.Data;
using CERVERICA.DTO.Stock;
using CERVERICA.DTO.Ventas;
using CERVERICA.Dtos;
using CERVERICA.Models;
using Microsoft.AspNetCore.Authorization;
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

        [HttpGet]
        public async Task<ActionResult<PedidoDTO>> ObtenerVentas()
        {
            var pedidos = await _context.Ventas
    .Select(p => new PedidoDTO
    {
        Id = p.Id,
        FechaVenta = p.FechaVenta,
        TotalCervezas = p.Total,
        MetodoEnvio = p.MetodoEnvio,
        MetodoPago = p.MetodoPago,
        NumeroTarjeta = p.NumeroTarjeta,
        EstatusVenta = p.EstatusVenta,
        MontoVenta = _context.DetallesVenta
            .Where(d => d.IdVenta == p.Id)
            .Sum(d => d.MontoVenta),
        ProductosPedido = _context.DetallesVenta
            .Where(d => d.IdVenta == p.Id)
            .Select(d => new DetalleVentaInformacionDTO
            {
                Id = d.Id,
                Cantidad = d.Cantidad,
                Pack = d.Pack,
                IdStock = d.IdStock,
                MontoVenta = d.MontoVenta,
                CostoUnitario = d.MontoVenta / d.Cantidad,
                Stock = _context.Stocks
                    .Where(s => s.Id == d.IdStock)
                    .Select(s => new StockDTO
                    {
                        Id = s.Id,
                        IdReceta = s.IdReceta,
                        Receta = s.Receta
                    })
                    .FirstOrDefault()
            })
            .ToArray()
    })
    .ToListAsync();


            if (pedidos == null)
            {
                return NotFound();
            }

            return Ok(pedidos);
        }

        [HttpGet("pedidos/{id}")]
        public async Task<ActionResult<PedidoDTO>> ObtenerPedido(int id)
        {
            var pedido = await _context.Ventas
    .Where(p => p.Id == id)
    .Select(p => new PedidoDTO
    {
        Id = p.Id,
        FechaVenta = p.FechaVenta,
        TotalCervezas = p.Total,
        MetodoEnvio = p.MetodoEnvio,
        MetodoPago = p.MetodoPago,
        NumeroTarjeta = p.NumeroTarjeta,
        EstatusVenta = p.EstatusVenta,
        ProductosPedido = _context.DetallesVenta
            .Where(d => d.IdVenta == p.Id)
            .Select(d => new DetalleVentaInformacionDTO
            {
                Id = d.Id,
                Cantidad = d.Cantidad,
                Pack = d.Pack,
                IdStock = d.IdStock,
                MontoVenta = d.MontoVenta,
                CostoUnitario = d.MontoVenta / d.Cantidad,
                Stock = _context.Stocks
                    .Where(s => s.Id == d.IdStock)
                    .Select(s => new StockDTO
                    {
                        Id = s.Id,
                        IdReceta = s.IdReceta,
                        Receta = s.Receta // Asumiendo que tienes una relación de navegación a la entidad Receta
                    })
                    .FirstOrDefault()
            })
            .ToArray()
    })
    .FirstOrDefaultAsync();


            if (pedido == null)
            {
                return NotFound();
            }

            return Ok(pedido);
        }

        [HttpGet("pedidos-usuario")]
        public async Task<List<PedidoDTO>> obtenerPedidosUsuario()
        {
            var idUsuario = User.FindFirstValue(ClaimTypes.NameIdentifier);
            Console.WriteLine("idUsuario");
            Console.WriteLine(idUsuario);

            return await _context.Ventas
                .Where(venta => venta.IdUsuario == idUsuario)
                 .OrderByDescending(venta => venta.Id)
                .Select(
                p => new PedidoDTO
                {
                    Id = p.Id,
                    FechaVenta = p.FechaVenta,
                    TotalCervezas = p.Total,
                    MetodoPago = p.MetodoPago,
                    MetodoEnvio = p.MetodoEnvio,
                    MontoVenta = _context.DetallesVenta
                    .Where(d => d.IdVenta == p.Id)
                    .Sum(d => d.MontoVenta),
                    EstatusVenta = p.EstatusVenta
                }).ToListAsync();
        }

        [HttpGet("pedidos")]
        public async Task<List<PedidoDTO>> obtenerPedidos()
        {
            return await _context.Ventas.Where(p => p.EstatusVenta == EstatusVenta.Recibido || p.EstatusVenta == EstatusVenta.Empaquetando).Select(
                p => new PedidoDTO
                {
                    Id = p.Id,
                    FechaVenta = p.FechaVenta,
                    TotalCervezas = p.Total,
                    MetodoEnvio = p.MetodoEnvio,
                    EstatusVenta = p.EstatusVenta
                }).ToListAsync();
        }

        [HttpGet("siguiente-estatus/{idPedido}")]
        public async Task<IActionResult> SiguienteEstatus(int idPedido)
        {
            // Busca el pedido en la base de datos
            var pedido = await _context.Ventas.FindAsync(idPedido);

            // Verifica si el pedido existe
            if (pedido == null)
            {
                return NotFound("Pedido no encontrado");
            }

            // Verifica si el estatus ya es 3
            if (pedido.EstatusVenta == EstatusVenta.Listo)
            {
                return BadRequest("El pedido ya se encuentra en el estatus máximo y no puede avanzar.");
            }

            // Cuenta el número de detalles de venta asociados al pedido
            var detalleCount = await _context.DetallesVenta.CountAsync(d => d.IdVenta == idPedido);

            // Actualiza el estatus del pedido
            if (detalleCount == 1)
            {
                pedido.EstatusVenta = EstatusVenta.Listo; // Suponiendo que "Listo" es un valor específico
            }
            else
            {
                pedido.EstatusVenta += 1; // Avanza al siguiente estatus
            }

            if (pedido.EstatusVenta == EstatusVenta.Empaquetando)
            {
                var notificacion = new Notificacion
                {
                    IdUsuario = pedido.IdUsuario,
                    Mensaje = $"¡Tu pedido con número #{pedido.Id} está en pleno proceso de empaquetado! Prepárate para disfrutar de una buena cerveza pronto. ¡Salud!",
                    Fecha = DateTime.Now,
                    Tipo = 8,
                    Visto = false
                };
                _context.Notificaciones.Add(notificacion);
            }

            if (pedido.EstatusVenta == EstatusVenta.Listo)
            {
                if (pedido.MetodoEnvio == MetodoEnvio.EnvioDomicilio)
                {
                    var notificacion = new Notificacion
                    {
                        IdUsuario = pedido.IdUsuario,
                        Mensaje = $"¡Tu pedido con número #{pedido.Id} está listo para ser entregado a domicilio! Prepárate para recibir tus cervezas y disfrutar de un excelente momento en casa",
                        Fecha = DateTime.Now,
                        Tipo = 8,
                        Visto = false
                    };
                    _context.Notificaciones.Add(notificacion);
                }
                else
                {
                    var notificacion = new Notificacion
                    {
                        IdUsuario = pedido.IdUsuario,
                        Mensaje = $"¡Tu pedido con número #{pedido.Id} está listo para recogerse! Prepárate para recoger tus cervezas y disfrutar de un excelente momento",
                        Fecha = DateTime.Now,
                        Tipo = 8,
                        Visto = false
                    };
                    _context.Notificaciones.Add(notificacion);
                }
            }

            // Guarda los cambios en la base de datos
            _context.Ventas.Update(pedido);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpGet("siguiente-estatus-landing/{idPedido}")]
        public async Task<IActionResult> SiguienteEstatusLanding(int idPedido)
        {
            var pedido = await _context.Ventas.FindAsync(idPedido);

            if (pedido == null)
            {
                return NotFound("Pedido no encontrado");
            }

            if (pedido.EstatusVenta == EstatusVenta.Listo)
            {
                return BadRequest("El pedido ya se encuentra en el estatus máximo y no puede avanzar.");
            }

            pedido.EstatusVenta += 1;

            if (pedido.EstatusVenta == EstatusVenta.Empaquetando)
            {
                var notificacion = new Notificacion
                {
                    IdUsuario = pedido.IdUsuario,
                    Mensaje = $"¡Tu pedido con número #{pedido.Id} está en pleno proceso de empaquetado! Prepárate para disfrutar de una buena cerveza pronto. ¡Salud!",
                    Fecha = DateTime.Now,
                    Tipo = 8,
                    Visto = false
                };
                _context.Notificaciones.Add(notificacion);
            }

            if (pedido.EstatusVenta == EstatusVenta.Listo)
            {
                if (pedido.MetodoEnvio == MetodoEnvio.EnvioDomicilio)
                {
                    var notificacion = new Notificacion
                    {
                        IdUsuario = pedido.IdUsuario,
                        Mensaje = $"¡Tu pedido con número #{pedido.Id} está listo para ser entregado a domicilio! Prepárate para recibir tus cervezas y disfrutar de un excelente momento en casa",
                        Fecha = DateTime.Now,
                        Tipo = 8,
                        Visto = false
                    };
                    _context.Notificaciones.Add(notificacion);
                }
                else
                {
                    var notificacion = new Notificacion
                    {
                        IdUsuario = pedido.IdUsuario,
                        Mensaje = $"¡Tu pedido con número #{pedido.Id} está listo para recogerse! Prepárate para recoger tus cervezas y disfrutar de un excelente momento",
                        Fecha = DateTime.Now,
                        Tipo = 8,
                        Visto = false
                    };
                    _context.Notificaciones.Add(notificacion);
                }
            }

            _context.Ventas.Update(pedido);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpGet("anterior-estatus/{idPedido}")]
        public async Task<IActionResult> AnteriorEstatus(int idPedido)
        {
            // Busca el pedido en la base de datos
            var pedido = await _context.Ventas.FindAsync(idPedido);

            // Verifica si el pedido existe
            if (pedido == null)
            {
                return NotFound("Pedido no encontrado");
            }

            // Verifica si el estatus ya es 1
            if (pedido.EstatusVenta == EstatusVenta.Recibido)
            {
                return BadRequest("El pedido ya se encuentra en el estatus mínimo y no puede retroceder.");
            }

            pedido.EstatusVenta -= 1; // Retrocede al estatus anterior

            // Guarda los cambios en la base de datos
            _context.Ventas.Update(pedido);
            await _context.SaveChangesAsync();

            return NoContent();
        }



        [HttpGet("cliente")]
        public async Task<List<VentasClienteDto>> GetVentasCliente()
        {
            //obtener el id del usuario actual
            var id = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var compras = await _context.Ventas.Where(p => p.IdUsuario == id).Select(
                p => new VentasClienteDto
                {
                    Id = p.Id,
                    FechaVenta = p.FechaVenta,
                    Total = p.Total,
                    MetodoEnvio = p.MetodoEnvio,
                    MetodoPago = p.MetodoPago,
                    EstatusVenta = p.EstatusVenta
                }).ToListAsync();

            //ordenar por fecha descendente
            compras = compras.OrderByDescending(p => p.FechaVenta).ToList();

            return compras;
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

            // Agrupa los detalles por idReceta y calcula la cantidad total de cervezas por receta
            var recetasAgrupadas = dto.Detalles
                .GroupBy(d => d.IdReceta)
                .Select(g => new
                {
                    IdReceta = g.Key,
                    CantidadTotalCervezas = g.Sum(d => d.Pack * d.Cantidad)
                }).ToList(); // Convierte a lista para manipular los datos

            // Crea un diccionario para almacenar la cantidad total requerida por receta
            var cantidadesRestantes = recetasAgrupadas.ToDictionary(r => r.IdReceta, r => r.CantidadTotalCervezas);

            foreach (var recetaAgrupada in recetasAgrupadas)
            {
                var stocks = await _context.Stocks
                    .Where(s => s.IdReceta == recetaAgrupada.IdReceta && s.Cantidad > 0)
                    .OrderBy(s => s.FechaEntrada)
                    .ToListAsync();

                var cantidadTotalDisponible = stocks.Sum(s => s.Cantidad);

                if (cantidadTotalDisponible < recetaAgrupada.CantidadTotalCervezas)
                {
                    // Encuentra todos los productos del carrito que coinciden con la receta y el usuario
                    var productosCarritoEliminar = await _context.ProductosCarrito
                        .Where(productoCarrito => productoCarrito.IdUsuario == currentUserId && productoCarrito.IdReceta == recetaAgrupada.IdReceta)
                        .OrderBy(productoCarrito => productoCarrito.CantidadLote) // Ordena por tamaño de paquete
                        .ToListAsync();

                    foreach (var productoCarrito in productosCarritoEliminar)
                    {
                        var receta = await _context.Recetas.Where(r => r.Id == recetaAgrupada.IdReceta).FirstOrDefaultAsync();
                        productosEliminados.Add(receta.Nombre + " - " + productoCarrito.CantidadLote + " Pack");
                        _context.ProductosCarrito.Remove(productoCarrito);

                        // Ajusta la cantidad total requerida en el diccionario
                        cantidadesRestantes[recetaAgrupada.IdReceta] -= productoCarrito.CantidadLote * productoCarrito.Cantidad;

                        if (cantidadesRestantes[recetaAgrupada.IdReceta] <= 0)
                        {
                            break; // Detén la eliminación si ya no es necesario
                        }
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

                        return StatusCode(410, (new { message = $"No hay suficiente stock para {nombreReceta}. Cervezas Requeridas: {cantidadTotalRequerida:f2}, Disponibles: {cantidadTotalDisponible}" }));

                    }

                    while (detalle.Cantidad > 0)
                    {
                        var stock = stocks.FirstOrDefault();
                        if (stock == null)
                        {
                            var nombreReceta = await _context.Recetas.Where(r => r.Id == recetaId).Select(r => r.Nombre).FirstOrDefaultAsync();
                            return BadRequest(new { message = $"No hay suficiente stock de la cerveza {nombreReceta}" });
                        }

                        // Asegurarse de que el stock puede cubrir al menos un pack
                        if (stock.Cantidad < detalle.Pack)
                        {
                            // Remover este stock de la lista y continuar con el siguiente
                            stocks.Remove(stock);
                            continue;
                        }

                        Debug.WriteLine("cantidad en stock" + stock.Cantidad);
                        Debug.WriteLine("tamaño del pack " + detalle.Pack);
                        var reduccionMaximaPacks = (int)(stock.Cantidad / detalle.Pack);
                        Debug.WriteLine("reduccion maxima " + reduccionMaximaPacks);
                        if (reduccionMaximaPacks == 0)
                        {
                            break;
                        }

                        var cantidadADescontar = Math.Min(reduccionMaximaPacks, (detalle.Cantidad));
                        stock.Cantidad -= cantidadADescontar * detalle.Pack;
                        detalle.Cantidad -= cantidadADescontar;

                        var receta = await _context.Recetas.FirstOrDefaultAsync(r => r.Id == recetaId);

                        if (receta == null)
                        {
                            return NotFound(new { message = $"No se encontró la receta" });
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
                            return BadRequest(new { message = $"Pack de {detalle.Pack} de {receta.Nombre} no se encuentra disponible para la venta." });

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

                // Venta creada correctamente se eliminan los productos del carrito
                _context.ProductosCarrito.RemoveRange(_context.ProductosCarrito.Where(pc => pc.IdUsuario == currentUserId));

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                try
                {
                    var notificacion = new Notificacion
                    {
                        IdUsuario = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? currentUserId,
                        Mensaje = $"Se realizó una compra :) Disfruta tus cervezas bien frías",
                        Fecha = DateTime.Now,
                        Tipo = 8,
                        Visto = false
                    };
                    _context.Notificaciones.Add(notificacion);

                    await _context.SaveChangesAsync();
                }
                catch (Exception ex)
                {

                }

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

            try
            {
                var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var notificacion = new Notificacion
                {
                    IdUsuario = currentUserId,
                    Mensaje = $"Se registro merma en un stock por {stock.Merma} {stock.TipoEnvase}s",
                    Fecha = DateTime.Now,
                    Tipo = 8,
                    Visto = false
                };
                _context.Notificaciones.Add(notificacion);

                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {

            }

            return Ok(stock);
        }
    }
}
