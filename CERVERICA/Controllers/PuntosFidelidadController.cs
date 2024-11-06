using CERVERICA.Data;
using CERVERICA.DTO.PuntosFidelidad;
using CERVERICA.Dtos;
using CERVERICA.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace CERVERICA.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class PuntosFidelidadController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;

        public PuntosFidelidadController(UserManager<ApplicationUser> userManager, ApplicationDbContext db)
        {
            _userManager = userManager;
            _db = db;
        }

        [HttpGet("obtener-puntos-fidelidad")]
        public async Task<ActionResult<PuntosFidelidadDto>> ObtenerPuntosFidelidad()
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

            var puntosFidelidad = await _db.PuntosFidelidad
                .FirstOrDefaultAsync(p => p.IdUsuario == user.Id);

            if (puntosFidelidad == null)
            {
                return NotFound(new AuthResponseDto
                {
                    IsSuccess = false,
                    Message = "No se encontraron puntos de fidelidad para el usuario"
                });
            }

            var puntosFidelidadDto = new PuntosFidelidadDto
            {
                Id = puntosFidelidad.Id,
                IdUsuario = puntosFidelidad.IdUsuario,
                PuntosAcumulados = puntosFidelidad.PuntosAcumulados ?? 0,
                PuntosRedimidos = puntosFidelidad.PuntosRedimidos ?? 0,
                PuntosDisponibles = puntosFidelidad.PuntosDisponibles ?? 0,
                FechaUltimaActualizacion = puntosFidelidad.FechaUltimaActualizacion
            };

            return Ok(puntosFidelidadDto);
        }

        [HttpGet("obtener-transacciones")]
        public async Task<ActionResult<List<TransaccionPuntosDto>>> ObtenerTransacciones()
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

            var transacciones = await _db.TransaccionesPuntos
                .Where(t => t.IdUsuario == user.Id)
                .OrderByDescending(t => t.FechaTransaccion)
                .Select(t => new TransaccionPuntosDto
                {
                    Id = t.Id,
                    Puntos = t.Puntos,
                    TipoTransaccion = t.TipoTransaccion,
                    FechaTransaccion = t.FechaTransaccion,
                    Descripcion = t.Descripcion
                }).ToListAsync();

            return Ok(transacciones);
        }

        [HttpPost("registrar-regla-puntos")]
        public async Task<ActionResult<ReglaPuntos>> RegistrarReglaPuntos([FromBody] ReglaPuntos reglaPuntos)
        {
            if (reglaPuntos == null)
            {
                return BadRequest(new AuthResponseDto
                {
                    IsSuccess = false,
                    Message = "Datos inválidos"
                });
            }

            // Buscar si ya existe un registro en la tabla
            var reglaExistente = await _db.ReglasPuntos.FirstOrDefaultAsync();

            if (reglaExistente == null)
            {
                // Si no existe, asignar la fecha de modificación y agregar el nuevo registro
                await _db.ReglasPuntos.AddAsync(reglaPuntos);
                await _db.SaveChangesAsync();

                return Created("api/puntosfidelidad/registrar-regla-puntos", reglaPuntos);
            }
            else
            {
                // Si ya existe, redirigir a la actualización
                return await ActualizarReglaPuntos(reglaPuntos);
            }
        }

        [HttpPut("actualizar-regla-puntos")]
        public async Task<ActionResult<ReglaPuntos>> ActualizarReglaPuntos([FromBody] ReglaPuntos reglaPuntos)
        {
            if (reglaPuntos == null)
            {
                return BadRequest(new AuthResponseDto
                {
                    IsSuccess = false,
                    Message = "Datos inválidos"
                });
            }

            // Buscar el registro existente
            var reglaExistente = await _db.ReglasPuntos.FirstOrDefaultAsync();

            if (reglaExistente == null)
            {
                return NotFound(new AuthResponseDto
                {
                    IsSuccess = false,
                    Message = "No se encontró la regla de puntos para actualizar"
                });
            }

            // Actualizar los campos necesarios
            reglaExistente.ValorMXNPunto = reglaPuntos.ValorMXNPunto;
            reglaExistente.MontoMinimo = reglaPuntos.MontoMinimo;
            reglaExistente.PorcentajeConversion = reglaPuntos.PorcentajeConversion;
            reglaExistente.FechaModificacion = reglaPuntos.FechaModificacion;

            _db.ReglasPuntos.Update(reglaExistente);
            await _db.SaveChangesAsync();

            return Ok(reglaExistente);
        }

        [HttpGet("obtener-regla-puntos")]
        public async Task<ActionResult<ReglaPuntos>> ObtenerReglaPuntos()
        {
            var reglaPuntos = await _db.ReglasPuntos.FirstOrDefaultAsync();

            if (reglaPuntos == null)
            {
                return NotFound(new AuthResponseDto
                {
                    IsSuccess = false,
                    Message = "No se encontró la regla de puntos"
                });
            }

            return Ok(reglaPuntos);
        }

    }
}
