using CERVERICA.Data;
using CERVERICA.DTO.Stock;
using CERVERICA.DTO.Ventas;
using CERVERICA.Dtos;
using CERVERICA.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
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
        private readonly ILogger<VentasController> _logger;

        public VentasController(UserManager<ApplicationUser> userManager, ApplicationDbContext context, ILogger<VentasController> logger)
        {
            _userManager = userManager;
            _context = context;
            _logger = logger;

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
            Console.WriteLine("---------------------------");
            Console.WriteLine("3");
            Console.WriteLine("---------------------------");

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

            Console.WriteLine("---------------------------");
            Console.WriteLine("2");
            Console.WriteLine("---------------------------");

            var productosEliminados = new List<string>();

            // Agrupa los detalles por idReceta y calcula la cantidad total de cervezas por receta
            var recetasAgrupadas = dto.Detalles
                .GroupBy(d => d.IdReceta)
                .Select(g => new
                {
                    IdReceta = g.Key,
                    CantidadTotalCervezas = g.Sum(d => d.Pack * d.Cantidad)
                }).ToList();

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
                    var carritoUsuario = await _context.Carritos
                        .Where(c => c.IdUsuario == currentUserId)
                        .FirstOrDefaultAsync();

                    var productosCarritoEliminar = await _context.ProductosCarrito
                        .Where(productoCarrito => productoCarrito.IdCarrito == carritoUsuario.Id && productoCarrito.IdReceta == recetaAgrupada.IdReceta)
                        .OrderBy(productoCarrito => productoCarrito.CantidadPaquete)
                        .ToListAsync();

                    foreach (var productoCarrito in productosCarritoEliminar)
                    {
                        var receta = await _context.Recetas.Where(r => r.Id == recetaAgrupada.IdReceta).FirstOrDefaultAsync();
                        productosEliminados.Add(receta.Nombre + " - " + productoCarrito.CantidadPaquete + " Pack");
                        _context.ProductosCarrito.Remove(productoCarrito);

                        cantidadesRestantes[recetaAgrupada.IdReceta] -= productoCarrito.CantidadPaquete * productoCarrito.Cantidad;

                        if (cantidadesRestantes[recetaAgrupada.IdReceta] <= 0)
                        {
                            break;
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

            Console.WriteLine("---------------------------");
            Console.WriteLine("1");
            Console.WriteLine("---------------------------");

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var venta = new Venta
                {
                    IdUsuario = user.Id,
                    FechaVenta = DateTime.Now,
                    EstatusVenta = EstatusVenta.Recibido,
                    MetodoEnvio = dto.MetodoEnvio,
                    MetodoPago = dto.MetodoPago
                };

                if (dto.MetodoEnvio == MetodoEnvio.EnvioDomicilio)
                {
                    venta.NombrePersonaRecibe = dto.NombrePersonaRecibe;
                    venta.Calle = dto.Calle;
                    venta.NumeroCasa = dto.NumeroCasa;
                    venta.CodigoPostal = dto.CodigoPostal;
                    venta.Ciudad = dto.Ciudad;
                    venta.Estado = dto.Estado;
                }

                _context.Ventas.Add(venta);
                await _context.SaveChangesAsync();

                float totalVenta = 0;
                var detallesVentas = new List<DetalleVenta>();

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
                        return StatusCode(410, new { message = $"No hay suficiente stock para {nombreReceta}. Cervezas Requeridas: {cantidadTotalRequerida:f2}, Disponibles: {cantidadTotalDisponible}" });
                    }

                    while (detalle.Cantidad > 0)
                    {
                        var stock = stocks.FirstOrDefault();
                        if (stock == null)
                        {
                            var nombreReceta = await _context.Recetas.Where(r => r.Id == recetaId).Select(r => r.Nombre).FirstOrDefaultAsync();
                            return BadRequest(new { message = $"No hay suficiente stock de la cerveza {nombreReceta}" });
                        }

                        if (stock.Cantidad < detalle.Pack)
                        {
                            stocks.Remove(stock);
                            continue;
                        }

                        var reduccionMaximaPacks = (int)(stock.Cantidad / detalle.Pack);

                        if (reduccionMaximaPacks == 0)
                        {
                            break;
                        }

                        var cantidadADescontar = Math.Min(reduccionMaximaPacks, detalle.Cantidad);
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
                            IdReceta = recetaId,
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

                var carritoUsuario = await _context.Carritos
                    .Where(c => c.IdUsuario == currentUserId)
                    .FirstOrDefaultAsync();

                _context.ProductosCarrito.RemoveRange(_context.ProductosCarrito.Where(pc => pc.IdCarrito == carritoUsuario.Id));

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                var notificacion = new Notificacion
                {
                    IdUsuario = currentUserId,
                    Mensaje = "Se realizó una compra :) Disfruta tus cervezas bien frías",
                    Fecha = DateTime.Now,
                    Tipo = 8,
                    Visto = false
                };
                _context.Notificaciones.Add(notificacion);

                await _context.SaveChangesAsync();
                return Ok(venta);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, new { message = $"Error al crear la venta: {ex.Message}" });
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

        /**
         * Obtiene el total de ventas, y el dinero generado por el parametro proporcionado (semana, mes, año)
         */
        [AllowAnonymous]
        [HttpGet("total-ventas/{param}")]
        public async Task<IActionResult> GetTotalVentas(string param)
        {
            try
            {
                var ventas = new List<Venta>();
                var resultado = new { fecha = DateTime.Now, data = new List<object>(), total = 0.0 };

                switch (param)
                {
                    case "semana":
                        var startOfWeek = DateTime.Now.AddDays(-(int)DateTime.Now.DayOfWeek);
                        ventas = await _context.Ventas.Where(v => v.FechaVenta >= startOfWeek).ToListAsync();
                        resultado = new
                        {
                            fecha = DateTime.Now,
                            data = Enumerable.Range(0, 7).Select(i => new
                            {
                                date = startOfWeek.AddDays(i).ToString("dddd"),
                                monto = ventas.Where(v => v.FechaVenta.Date == startOfWeek.AddDays(i).Date).Sum(v => v.Total)
                            }).Cast<object>().ToList(),
                            total = ventas.Sum(v => (double)v.Total)
                        };
                        break;

                    case "mes":
                        var startOfMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                        ventas = await _context.Ventas.Where(v => v.FechaVenta >= startOfMonth).ToListAsync();
                        resultado = new
                        {
                            fecha = DateTime.Now,
                            data = Enumerable.Range(0, DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month)).Select(i => new
                            {
                                date = startOfMonth.AddDays(i).ToString("dd MMM"),
                                monto = ventas.Where(v => v.FechaVenta.Date == startOfMonth.AddDays(i).Date).Sum(v => v.Total)
                            }).Cast<object>().ToList(),
                            total = ventas.Sum(v => (double)v.Total)
                        };
                        break;

                    case "anio":
                        var startOfYear = new DateTime(DateTime.Now.Year, 1, 1);
                        ventas = await _context.Ventas.Where(v => v.FechaVenta >= startOfYear).ToListAsync();
                        resultado = new
                        {
                            fecha = DateTime.Now,
                            data = Enumerable.Range(0, 12).Select(i => new
                            {
                                date = startOfYear.AddMonths(i).ToString("MMMM"),
                                monto = ventas.Where(v => v.FechaVenta.Month == startOfYear.AddMonths(i).Month).Sum(v => v.Total)
                            }).Cast<object>().ToList(),
                            total = ventas.Sum(v => (double)v.Total)
                        };
                        break;

                    default:
                        return BadRequest("Parámetro no válido, solo se acepta (semana, mes o año)");
                }

                return Ok(resultado);
            }
            catch (Exception ex)
            {
                // Imprime la excepción en la consola
                Console.WriteLine($"Exception: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");

                // También puedes registrar la excepción usando el logger
                _logger.LogError(ex, "Error al obtener las ventas para el parámetro {Param}", param);

                // Devuelve una respuesta de error genérica
                return StatusCode(500, "Se produjo un error al procesar su solicitud.");
            }
        }



        [AllowAnonymous]
        [HttpGet("resumen-ventas")]
        public async Task<IActionResult> GetResumenVentas()
        {
            try
            {
                var ventas = await _context.Ventas.ToListAsync();

                var hoy = DateTime.Today;
                var inicioSemana = hoy.AddDays(-(int)hoy.DayOfWeek);
                var inicioMes = new DateTime(hoy.Year, hoy.Month, 1);
                var inicioAnio = new DateTime(hoy.Year, 1, 1);

                var totalVentasSemana = ventas.Where(v => v.FechaVenta >= inicioSemana).Sum(v => v.Total);
                var totalVentasMes = ventas.Where(v => v.FechaVenta >= inicioMes).Sum(v => v.Total);
                var totalVentasAnio = ventas.Where(v => v.FechaVenta >= inicioAnio).Sum(v => v.Total);

                var resultado = new
                {
                    semana = totalVentasSemana,
                    mes = totalVentasMes,
                    anio = totalVentasAnio
                };

                return Ok(resultado);
            }
            catch (Exception ex)
            {
                // Imprime la excepción en la consola
                Console.WriteLine($"Exception: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");

                // También puedes registrar la excepción usando el logger
                _logger.LogError(ex, "Error al obtener el resumen de ventas");

                // Devuelve una respuesta de error genérica
                return StatusCode(500, "Se produjo un error al procesar su solicitud.");
            }
        }




    }
}
