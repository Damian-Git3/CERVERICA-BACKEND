using CERVERICA.Data;
using CERVERICA.Dtos;
using CERVERICA.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Stripe;
using Stripe.Checkout;
using System.Net;
using System.Text.Json;

namespace CERVERICA.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class CarritoController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;

        public CarritoController(UserManager<ApplicationUser> userManager, ApplicationDbContext db)
        {
            _userManager = userManager;
            _db = db;
        }

        [HttpPost("agregar-producto-carrito")]
        public async Task<ActionResult<ProductoCarrito>> agregarProductoCarrito(AgregarProductoCarrito productoCarritoAgregar)
        {
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

            var nuevoProductoCarrito = new ProductoCarrito();
            nuevoProductoCarrito.IdUsuario = user.Id;
            nuevoProductoCarrito.IdReceta = productoCarritoAgregar.IdReceta;
            nuevoProductoCarrito.CantidadLote = productoCarritoAgregar.CantidadLote;
            nuevoProductoCarrito.Cantidad = productoCarritoAgregar.Cantidad;

            _db.ProductosCarrito.Add(nuevoProductoCarrito);
            await _db.SaveChangesAsync();

            var productoConReceta = await _db.ProductosCarrito
                .Where(f => f.Id == nuevoProductoCarrito.Id)
                .Include(f => f.Receta)
                .FirstOrDefaultAsync();

            return Ok(productoConReceta);
        }

        [HttpPost("actualizar-producto-carrito")]
        public async Task<ActionResult<ProductoCarrito>> ActualizarProductoCarrito(ActualizarProductoCarritoDTO productoCarritoActualizar)
        {
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

            // Buscar el producto en el carrito del usuario
            var productoEnCarrito = await _db.ProductosCarrito.FirstOrDefaultAsync(p =>
                p.IdUsuario == user.Id &&
                p.IdReceta == productoCarritoActualizar.IdReceta &&
                p.CantidadLote == productoCarritoActualizar.CantidadLote);

            if (productoEnCarrito is null)
            {
                return NotFound(new AuthResponseDto
                {
                    IsSuccess = false,
                    Message = "Producto en el carrito no encontrado"
                });
            }

            // Actualizar la cantidad
            productoEnCarrito.Cantidad = productoCarritoActualizar.Cantidad;

            _db.ProductosCarrito.Update(productoEnCarrito);
            await _db.SaveChangesAsync();

            var productoConReceta = await _db.ProductosCarrito
                .Where(f => f.Id == productoEnCarrito.Id)
                .Include(f => f.Receta)
                .FirstOrDefaultAsync();

            return Ok(productoConReceta);
        }

        [HttpPost("eliminar-producto-carrito")]
        public async Task<ActionResult<string>> EliminarProductoCarrito(EliminarProductoCarritoDTO productoCarritoEliminarDTO)
        {
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

            var productoCarritoEliminar = await _db.ProductosCarrito.FirstOrDefaultAsync(f => f.IdUsuario == user.Id && f.IdReceta == productoCarritoEliminarDTO.IdReceta && f.CantidadLote == productoCarritoEliminarDTO.CantidadLote);

            if (productoCarritoEliminar == null)
            {
                return NotFound("Favorito no encontrado.");
            }

            _db.ProductosCarrito.Remove(productoCarritoEliminar);

            await _db.SaveChangesAsync();

            return Ok(new { message = "Producto carrito eliminado exitosamente" });
        }

        [HttpGet("obtener-productos-carrito")]
        public async Task<ActionResult<ProductoCarrito[]>> obtenerProductosCarrito()
        {
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

            var productosCarrito = await _db.ProductosCarrito
            .Where(f => f.IdUsuario == user.Id)
            .Include(f => f.Receta)
            .ToListAsync();

            return Ok(productosCarrito);
        }

        [HttpGet("session-status")]
        public ActionResult SessionStatus([FromQuery] string session_id)
        {
            var sessionService = new SessionService();
            Session session = sessionService.Get(session_id);

            return Ok(new { status = session.Status, customer_email = session.CustomerDetails.Email });
        }

        [HttpPost("checkout")]
        public async Task<ActionResult> Checkout([FromBody] CrearVentaDto dto)
        {
            var idUsuarioPeticion = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var productosCarrito = await _db.ProductosCarrito
            .Where(f => f.IdUsuario == idUsuarioPeticion)
            .Include(f => f.Receta)
            .ToListAsync();

            var lineItems = productosCarrito.Select(productoCarrito =>
            {
                long precioPaquete = 0;

                switch (productoCarrito.CantidadLote)
                {
                    case 1:
                        precioPaquete = (long)(productoCarrito.Receta.PrecioPaquete1 * 100);
                        break;
                    case 6:
                        precioPaquete = (long)(productoCarrito.Receta.PrecioPaquete6 * 100);
                        break;
                    case 12:
                        precioPaquete = (long)(productoCarrito.Receta.PrecioPaquete12 * 100);
                        break;
                    case 24:
                        precioPaquete = (long)(productoCarrito.Receta.PrecioPaquete24 * 100);
                        break;
                }

                Console.WriteLine("Precio paquete" + precioPaquete);

                return new SessionLineItemOptions
                {
                    Quantity = productoCarrito.Cantidad,
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        Currency = "mxn",
                        UnitAmount = precioPaquete,
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = productoCarrito.Receta.Nombre + " - Paquete de " + productoCarrito.CantidadLote,
                            Images = new List<string> { productoCarrito.Receta.Imagen }
                        }
                    }
                };
            }).ToList();

            var dtoJson = JsonSerializer.Serialize(dto);

            var encodedDto = WebUtility.UrlEncode(dtoJson);

            var domain = "http://localhost:4200";
            var options = new SessionCreateOptions
            {
                LineItems = lineItems,
                Mode = "payment",
                UiMode = "embedded",
                ReturnUrl = $"{domain}/cerverica/carrito?venta={encodedDto}",
            };
            var service = new SessionService();
            Session session = service.Create(options);

            return Ok(new { clientSecret = session.ClientSecret });
        }
    }
}
