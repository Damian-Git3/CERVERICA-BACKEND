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
    public class ConfiguracionesGeneralesController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;

        public ConfiguracionesGeneralesController(UserManager<ApplicationUser> userManager, ApplicationDbContext db)
        {
            _userManager = userManager;
            _db = db;
        }

        [HttpPost("registrar-configuracion-general")]
        public async Task<ActionResult<ConfiguracionesGenerales>> RegistrarConfiguracionGeneral([FromBody] ConfiguracionesGenerales configuracion)
        {
            if (configuracion == null)
            {
                return BadRequest(new AuthResponseDto
                {
                    IsSuccess = false,
                    Message = "Datos inválidos"
                });
            }

            // Buscar si ya existe un registro en la tabla
            var configuracionExistente = await _db.ConfiguracionesGenerales.FirstOrDefaultAsync();

            if (configuracionExistente == null)
            {
                // Si no existe, asignar la fecha de modificación y agregar el nuevo registro
                await _db.ConfiguracionesGenerales.AddAsync(configuracion);
                await _db.SaveChangesAsync();

                return Created("api/configuracion-general/registrar-configuracion-general", configuracion);
            }
            else
            {
                // Si ya existe, redirigir a la actualización
                return await ActualizarConfiguracionGeneral(configuracion);
            }
        }

        [HttpPut("actualizar-configuracion-general")]
        public async Task<ActionResult<ConfiguracionesGenerales>> ActualizarConfiguracionGeneral([FromBody] ConfiguracionesGenerales configuracion)
        {
            if (configuracion == null)
            {
                return BadRequest(new AuthResponseDto
                {
                    IsSuccess = false,
                    Message = "Datos inválidos"
                });
            }

            // Buscar el registro existente
            var configuracionExistente = await _db.ConfiguracionesGenerales.FirstOrDefaultAsync();

            if (configuracionExistente == null)
            {
                return NotFound(new AuthResponseDto
                {
                    IsSuccess = false,
                    Message = "No se encontró la configuración general para actualizar"
                });
            }

            // Actualizar los campos necesarios
            configuracionExistente.MinimoCompraEnvioGratis = configuracion.MinimoCompraEnvioGratis;
            configuracionExistente.PromocionesAutomaticas = configuracion.PromocionesAutomaticas;
            configuracionExistente.NotificacionPromocionesWhatsApp = configuracion.NotificacionPromocionesWhatsApp;
            configuracionExistente.NotificacionPromocionesEmail = configuracion.NotificacionPromocionesEmail;
            configuracionExistente.TiempoRecordatorioCarritoAbandonado = configuracion.TiempoRecordatorioCarritoAbandonado;
            configuracionExistente.TiempoRecordatorioRecomendacionUltimaCompra = configuracion.TiempoRecordatorioRecomendacionUltimaCompra;
            configuracionExistente.FechaModificacion = configuracion.FechaModificacion;
            configuracionExistente.FrecuenciaReclasificacionClientes = configuracion.FrecuenciaReclasificacionClientes;
            configuracionExistente.FrecuenciaMinimaMensualClienteFrecuente = configuracion.FrecuenciaMinimaMensualClienteFrecuente;
            configuracionExistente.TiempoSinComprasClienteInactivo = configuracion.TiempoSinComprasClienteInactivo;

            _db.ConfiguracionesGenerales.Update(configuracionExistente);
            await _db.SaveChangesAsync();

            return Ok(configuracionExistente);
        }

        [HttpGet("obtener-configuracion-general")]
        public async Task<ActionResult<ConfiguracionesGenerales>> ObtenerConfiguracionGeneral()
        {
            var configuracion = await _db.ConfiguracionesGenerales.FirstOrDefaultAsync();

            if (configuracion == null)
            {
                return NotFound(new AuthResponseDto
                {
                    IsSuccess = false,
                    Message = "No se encontró la configuración general"
                });
            }

            return Ok(configuracion);
        }
    }
}
