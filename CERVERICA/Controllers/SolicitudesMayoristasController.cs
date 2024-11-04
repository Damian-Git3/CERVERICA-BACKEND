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

        [HttpGet("obtener-solicitudes-mayoristas")]
        public async Task<ActionResult<SolicitudMayorista>> obtenerSolicitudesMayoristas()

        {
            var idUsuario = HttpContext.Items["idUsuario"] as string;

            var solicitudesMayoristas = await _db.SolicitudesMayorista
            .Where(s => s.IdAgente == idUsuario)
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

    }
}
