using CERVERICA.Models;
using CERVERICA.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using CERVERICA.Dtos;

namespace CERVERICA.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ConfiguracionVentasMayoreoController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;

        public ConfiguracionVentasMayoreoController(UserManager<ApplicationUser> userManager, ApplicationDbContext db)
        {
            _userManager = userManager;
            _db = db;
        }

        [HttpPost("registrar-configuracion")]
        public async Task<ActionResult<ConfiguracionVentasMayoreo>> RegistrarConfiguracion([FromBody] ConfiguracionVentasMayoreo configuracion)
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
            var configuracionExistente = await _db.ConfiguracionesVentasMayoreo.FirstOrDefaultAsync();

            if (configuracionExistente == null)
            {
                // Si no existe, agregar el nuevo registro
                await _db.ConfiguracionesVentasMayoreo.AddAsync(configuracion);
                await _db.SaveChangesAsync();

                return Created("api/configuracion/registrar-configuracion", configuracion);
            }
            else
            {
                // Si ya existe, redirigir a la actualización
                return await ActualizarConfiguracion(configuracion);
            }
        }

        [HttpPut("actualizar-configuracion")]
        public async Task<ActionResult<ConfiguracionVentasMayoreo>> ActualizarConfiguracion([FromBody] ConfiguracionVentasMayoreo configuracion)
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
            var configuracionExistente = await _db.ConfiguracionesVentasMayoreo.FirstOrDefaultAsync();

            if (configuracionExistente == null)
            {
                return NotFound(new AuthResponseDto
                {
                    IsSuccess = false,
                    Message = "No se encontró la configuración de ventas al por mayor para actualizar"
                });
            }

            // Actualizar los campos necesarios
            configuracionExistente.PlazoMaximoPago = configuracion.PlazoMaximoPago;
            configuracionExistente.PagosMensuales = configuracion.PagosMensuales;
            configuracionExistente.MontoMinimoMayorista = configuracion.MontoMinimoMayorista;
            configuracionExistente.FechaModificacion = configuracion.FechaModificacion;

            _db.ConfiguracionesVentasMayoreo.Update(configuracionExistente);
            await _db.SaveChangesAsync();

            return Ok(configuracionExistente);
        }

        [HttpGet("obtener-configuracion")]
        public async Task<ActionResult<ConfiguracionVentasMayoreo>> ObtenerConfiguracion()
        {
            var configuracion = await _db.ConfiguracionesVentasMayoreo.FirstOrDefaultAsync();

            if (configuracion == null)
            {
                return NotFound(new AuthResponseDto
                {
                    IsSuccess = false,
                    Message = "No se encontró la configuración de ventas al por mayor"
                });
            }

            return Ok(configuracion);
        }
    }
}
