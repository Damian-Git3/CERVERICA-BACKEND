using CERVERICA.Data;
using CERVERICA.DTO.SolicitudesMayoristas;
using CERVERICA.Dtos;
using CERVERICA.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace CERVERICA.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SolicitudesMayoristasController : ControllerBase
    {
        private readonly ApplicationDbContext _db;

        public SolicitudesMayoristasController(UserManager<ApplicationUser> userManager, ApplicationDbContext db)
        {
            _db = db;
        }

        [HttpPost]
        public async Task<ActionResult<SolicitudMayorista>> crearSolicitudMayorista()
        {
            var idUsuario = HttpContext.Items["idUsuario"] as string;
            var mayorista = await _db.ClientesMayoristas.Where(m => m.IdUsuario == idUsuario).FirstAsync();

            var nuevaSolicitudMayorista = new SolicitudMayorista
            {
                FechaInicio = DateTime.Now,
                Estatus = EstatusSolicitudMayorista.Confirmando,
                IdMayorista = mayorista.Id,
                IdAgente = mayorista.IdAgenteVenta
            };

            _db.SolicitudesMayorista.Add(nuevaSolicitudMayorista);

            await _db.SaveChangesAsync();

            return Ok("Solicitud generada correctamente");
        }

        [HttpGet("obtener-solicitudes-agente")]
        public async Task<ActionResult<SolicitudMayorista>> obtenerSolicitudesAgente()

        {
            var idUsuario = HttpContext.Items["idUsuario"] as string;

            var solicitudesMayoristas = await _db.SolicitudesMayorista
            .Where(s => s.IdAgente == idUsuario)
            .Include(s => s.Mayorista)
            .ToListAsync();

            return Ok(solicitudesMayoristas);
        }

        [HttpGet("obtener-solicitudes-mayorista")]
        public async Task<ActionResult<SolicitudMayorista>> obtenerSolicitudesMayorista()

        {
            var idUsuario = HttpContext.Items["idUsuario"] as string;
            var mayorista = await _db.ClientesMayoristas.Where(m => m.IdUsuario == idUsuario).FirstAsync();

            var solicitudesMayoristas = await _db.SolicitudesMayorista
            .Where(s => s.IdMayorista == mayorista.Id)
            .Include(s => s.Mayorista)
            .ToListAsync();

            return Ok(solicitudesMayoristas);
        }


        [HttpPost("avanzar-siguiente-estatus")]
        public async Task<ActionResult> AvanzarSiguienteEstatus([FromBody] AvanzarSiguienteEstatusSolicitudMayorista avanzarSiguienteEstatusDTO)
        {
            Console.WriteLine(avanzarSiguienteEstatusDTO);

            if (avanzarSiguienteEstatusDTO == null)
            {
                return BadRequest("Solicitud o estatus inválido.");
            }

            var solicitud = await _db.SolicitudesMayorista.FindAsync(avanzarSiguienteEstatusDTO.IdSolicitud);
            if (solicitud == null)
            {
                return NotFound("Solicitud no encontrada.");
            }

            solicitud.Estatus = (EstatusSolicitudMayorista)avanzarSiguienteEstatusDTO.NuevoEstatus;
            await _db.SaveChangesAsync();

            return Ok("Estatus actualizado exitosamente.");
        }

        [HttpPost("cancelar-solicitud")]
        public async Task<ActionResult> CancelarSolicitudMayorista([FromBody] CancelarSolicitudMayoristaDTO cancelarSolicitudDTO)
        {
            if (cancelarSolicitudDTO == null)
            {
                return BadRequest("Datos de cancelación inválidos.");
            }

            var solicitud = await _db.SolicitudesMayorista.FindAsync(cancelarSolicitudDTO.IdSolicitud);
            if (solicitud == null)
            {
                return NotFound("Solicitud no encontrada.");
            }

            solicitud.Estatus = EstatusSolicitudMayorista.Cancelado;
            solicitud.mensajeRechazo = cancelarSolicitudDTO.MensajeRechazo;

            await _db.SaveChangesAsync();

            return Ok("Solicitud cancelada exitosamente.");
        }


        [HttpGet("obtener-carrito-solicitud/{idSolicitud}")]
        public async Task<ActionResult<ProductoCarrito[]>> obtenerProductosCarrito(int idSolicitud)
        {
            var solicitudMayorista = await _db.SolicitudesMayorista
            .Where(c => c.Id == idSolicitud)
            .FirstOrDefaultAsync();

            var clienteMayorista = await _db.ClientesMayoristas
            .Where(c => c.Id == solicitudMayorista.IdMayorista)
            .FirstOrDefaultAsync();

            var carritoUsuario = await _db.Carritos
            .Where(c => c.IdUsuario == clienteMayorista.IdUsuario)
            .FirstOrDefaultAsync();

            var productosCarrito = await _db.ProductosCarrito
            .Where(pc => pc.IdCarrito == carritoUsuario.Id)
            .Include(f => f.Receta)
            .ToListAsync();

            return Ok(productosCarrito);
        }
    }
}
