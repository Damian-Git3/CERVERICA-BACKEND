using CERVERICA.Data;
using CERVERICA.Dtos;
using CERVERICA.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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

            bool hayInventario = true;
            //verificar que hay suficiente inventario para cada producto para crear una venta ligada al pedido
            foreach (var producto in pedidoMayoristaDTO.ListaCervezas)
            {
                var receta = await _context.Recetas.FindAsync(producto.IdReceta);
                
                //consultar los stocks de la cerveza y sumar sus cantidades
                var CantidadDisponible = 0;
                foreach (var stock in receta.Stocks)
                {
                    CantidadDisponible += stock.Cantidad;
                }

                //si no hay suficiente inventario para algun producto crear el pedido con estatus 2 y sin ligar a venta
                if (CantidadDisponible < producto.Cantidad)
                {
                    hayInventario = false;
                    //generar la solicitud de produccion
                }
            }

            //si no hay suficiente inventario para algun producto crear el pedido con estatus 2 y sin ligar a venta
            //crear el pedido sin ligar a venta
            if (hayInventario == false)
            {
                pedido.Estatus = 2;
            }

            _context.PedidosMayoreo.Add(pedido);
            await _context.SaveChangesAsync();

            //crear los pagos ligados al pedido
            for (int i = 0; i < numeroPagos; i++)
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


            //si si hay suficiente stock crear la venta ligada al pedido

            //guardar los cambios
            await _context.SaveChangesAsync();

            return Ok(pedido);
        }
    }
}
