using CERVERICA.Data;
using CERVERICA.Dtos;
using CERVERICA.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestSharp;
using System.Security.Claims;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace CERVERICA.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class PedidosMayoristasController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public PedidosMayoristasController(ApplicationDbContext context)
        {
            _context = context;
        }

        //POST: api/pedidoMayorista
        //autorizar solo a los roles de agente
        [Authorize(Roles = "Agente")]
        [HttpPost("crear-pedido")]
        public async Task<ActionResult<PedidoMayoreo>> crearPedidoMayoreo(PedidoMayoristaInsertDto pedidoMayoristaDTO)
        {
            //Obtener el agente que esta asignado al cliente mayorista
            //Obtener el cliente mayorista
            var clienteMayorista = await _context.ClientesMayoristas.FindAsync(pedidoMayoristaDTO.IdMayorista);

            var idAgente = clienteMayorista.IdAgenteVenta;

            //obtener la fecha actual
            var fechaActual = DateTime.Now;

            //calcular la fecha limite sumando el plazo meses al dia actual
            var fechaLimite = fechaActual.AddMonths(pedidoMayoristaDTO.PlazoMeses);

            //el numero de pagos es igual al plazo en meses
            var numeroPagos = pedidoMayoristaDTO.PlazoMeses;

            //calcular el monto total multiplicando el precio unitario base mayoreo de cada producto por la cantidad de cada producto
            var total = 0.0;
            foreach (var producto in pedidoMayoristaDTO.ListaCervezas)
            {
                var receta = await _context.Recetas.FindAsync(producto.IdReceta);
                //si no hay receta saltar la iteracion
                if (receta == null)
                {
                    //retornar un error diciendo que no existe el producto
                    return BadRequest("No existe la id de cerveza proporcionada");

                }
                //si no hay un precio unitario base mandar error
                if (receta.PrecioUnitarioBaseMayoreo == 0 || receta.PrecioUnitarioBaseMayoreo==null)
                {
                    return BadRequest("No se ha fijado un precio base para la cerveza");
                }
                total += (double)receta.PrecioUnitarioBaseMayoreo * producto.Cantidad;
            }


            //calcular el monto por pago dividiendo el monto total entre el numero de pagos
            var montoPorPago = total / numeroPagos;

            var pedido = new PedidoMayoreo
            {
                IdMayorista = pedidoMayoristaDTO.IdMayorista,
                IdAgenteVenta = idAgente,
                FechaInicio = fechaActual,
                NumeroPagos = numeroPagos,
                FechaLimite = fechaLimite,
                MontoTotal = (float)total,
                MontoPorPago = (float)montoPorPago,
                Observaciones = pedidoMayoristaDTO.Observaciones,
                Estatus = 1
            };

            _context.PedidosMayoreo.Add(pedido);
            await _context.SaveChangesAsync();

            //crear los pagos ligados al pedido
            for (int i = 1; i <= numeroPagos; i++)
            {
                var pago = new Pago
                {
                    IdPedidoMayoreo = pedido.Id,
                    IdMayorista = pedido.IdMayorista,
                    Comprobante = "",
                    FechaVencimiento = fechaActual.AddMonths(i),
                    Monto = (float)montoPorPago,
                    Estatus = 1
                };
                _context.Pagos.Add(pago);
            }
            await _context.SaveChangesAsync();

            //crear las producciones ligadas al pedido

            var cantidadRecetas = pedidoMayoristaDTO.ListaCervezas.Count;

            foreach (var producto in pedidoMayoristaDTO.ListaCervezas)
            {
                //obtener la receta
                var receta = await _context.Recetas.FindAsync(producto.IdReceta);

                //calcular las botellas fabricadas con los litros estimados de la receta
                var botellas = Math.Floor((receta.LitrosEstimados * 0.98) / 0.355);

                //calcular las tandas minimas para la cantidad requerida
                var tandas = (int)Math.Ceiling(producto.Cantidad / botellas);

                //calcular el costo de produccion de la receta
                var precioFijo = producto.Cantidad * receta.PrecioUnitarioBaseMayoreo;

                //hacer las solicitudes de produccion con estatus 10 para indicar que estan pendientes de conseguir los insumos
                //cuando se consigan los insumos se intenta hacer la produccion y se cambia el estatus a 1 y no se permite rechazar o posponer

                //encontrar la id del rol operador
                var idOperador = _context.Roles.Where(r => r.Name == "Operador").FirstOrDefault().Id;

                //encontrar el id del usuario con el rol operador
                var operador = _context.UserRoles.Where(ur => ur.RoleId == idOperador).FirstOrDefault().UserId;

                var produccion = new Produccion
                {
                    FechaProduccion = DateTime.Now,
                    FechaProximoPaso = DateTime.Now,
                    Mensaje = "Es de un pedido mayorista",
                    Estatus = 10, //pedido mayorista
                    NumeroTandas = tandas,
                    IdReceta = producto.IdReceta,
                    FechaSolicitud = DateTime.Now,
                    IdUsuarioSolicitud = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "a0a0aaa0-0000-0aa0-0000-00aaa0aa0a00",
                    IdUsuarioProduccion = operador,
                    Paso = 0,
                    EsMayorista = true,
                    PrecioMayoristaFijado = precioFijo,
                    IdPedidoMayoreo = pedido.Id,
                    CantidadMayoristaRequerida = producto.Cantidad,
                    StocksRequeridos = cantidadRecetas,
                };
                _context.Producciones.Add(produccion);
                await _context.SaveChangesAsync();

            }

            //obtener el pedido con los pagos y las producciones
            var pedidoConPagos = await _context.PedidosMayoreo
                                                .Include(p => p.Pagos)
                                                .FirstOrDefaultAsync(p => p.Id == pedido.Id);

            return Ok(new { message= "Pedido mayorista realizado exitosamente, se han insertado los pagos y las producciones", ok=true});
        }


        // GET: api/pedidoMayorista
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PedidoMayoreo>>> GetPedidosMayoreo()
        {
            return await _context.PedidosMayoreo.ToListAsync();
        }

        // GET: api/pedidoMayorista/5
        [HttpGet("{id}")]
        public async Task<ActionResult<PedidoMayoreo>> GetPedidoMayoreo(int id)
        {
            var pedidoMayoreo = await _context.PedidosMayoreo.FindAsync(id);

            if (pedidoMayoreo == null)
            {
                return NotFound();
            }

            return pedidoMayoreo;
        }

        // GET pagos de un cliente mayorista
        [HttpGet("pagos/{idMayorista}")]
        public async Task<ActionResult<IEnumerable<Pago>>> GetPagosMayorista(int idMayorista)
        {
            var pagos = await _context.Pagos
                                    .Where(p => p.IdMayorista == idMayorista)
                                    .ToListAsync();

            return Ok(pagos);
        }

        // GET pagos de un cliente mayorista
        [Authorize(Roles = "Mayorista")]
        [HttpGet("pagos/cliente")]
        public async Task<ActionResult<IEnumerable<Pago>>> GetPagosMayoristaPropio()
        {
            var idUser = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var mayorista = await _context.ClientesMayoristas.FirstOrDefaultAsync(c => c.IdUsuario == idUser);

            var pagos = await _context.Pagos
                                    .Where(p => p.IdMayorista == mayorista.Id)
                                    .ToListAsync();

            return Ok(pagos);
        }

        // GET Pagos de un pedido mayorista
        [HttpGet("pagos/pedido/{idPedido}")]
        public async Task<ActionResult<IEnumerable<Pago>>> GetPagosPedido(int idPedido)
        {
            var pagos = await _context.Pagos
                                    .Where(p => p.IdPedidoMayoreo == idPedido)
                                    .ToListAsync();

            return Ok(pagos);
        }

        //GET lista de clientes mayoristas de un agente de venta
        [Authorize(Roles="Agente")]
        [HttpGet("mayoristas-asignados")]
        public async Task<ActionResult<IEnumerable<ClienteMayorista>>> GetClientesAgente()
        {
            var idAgente = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var clientes = await _context.ClientesMayoristas
                                        .Where(c => c.IdAgenteVenta == idAgente)
                                        .ToListAsync();

            return Ok(clientes);
        }

        //GET producciones de un pedido mayorista
        [HttpGet("producciones/{idPedido}")]
        public async Task<ActionResult<IEnumerable<Produccion>>> GetProduccionesPedido(int idPedido)
        {
            var producciones = await _context.Producciones
                                              .Where(p => p.IdPedidoMayoreo == idPedido)
                                              .ToListAsync();

            return Ok(producciones);
        }

        // PUT marcar pago como realizado
        [HttpPut("pago/{idPago}")]
        public async Task<ActionResult<Pago>> MarcarPagoRealizado(int idPago)
        {
            var pago = await _context.Pagos.FindAsync(idPago);

            if (pago == null)
            {
                return NotFound();
            }

            pago.Estatus = 2;
            pago.FechaPago = DateTime.Now;
            await _context.SaveChangesAsync();

            return Ok(pago);
        }
    }
}
