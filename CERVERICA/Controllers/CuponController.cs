using CERVERICA.Data;
using CERVERICA.DTO.Cupones;
using CERVERICA.DTO.PuntosFidelidad;
using CERVERICA.Dtos;
using CERVERICA.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CERVERICA.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CuponController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;

        public CuponController(UserManager<ApplicationUser> userManager, ApplicationDbContext db)
        {
            _userManager = userManager;
            _db = db;
        }

        [HttpPost("registrar-cupon")]
        public async Task<IActionResult> RegistrarCupon([FromBody] CuponDTO cuponDTO)
        {
            if (cuponDTO == null)
            {
                return BadRequest(new AuthResponseDto
                {
                    IsSuccess = false,
                    Message = "Datos inválidos"
                });
            }

            var cupon = new Cupones
            {
                FechaCreacion = cuponDTO.FechaCreacion,
                FechaExpiracion = cuponDTO.FechaExpiracion,
                Codigo = cuponDTO.Codigo,
                Tipo = cuponDTO.Tipo,
                Paquete = cuponDTO.Paquete,
                Cantidad = cuponDTO.Cantidad,
                Valor = cuponDTO.Valor,
                Usos = cuponDTO.Usos,
                MontoMaximo = cuponDTO.MontoMaximo,
                MontoMinimo = cuponDTO.MontoMinimo,
                Activo = cuponDTO.Activo
                // No se incluyen IdUsuario ni IdReceta en el DTO
            };

            await _db.Cupones.AddAsync(cupon);
            await _db.SaveChangesAsync();

            return Created("api/cupones/registrar-cupon", cupon);
        }

        [HttpPut("actualizar-cupon/{id}")]
        public async Task<ActionResult<Cupones>> ActualizarCupon(int id, [FromBody] CuponDTO cuponDTO)
        {
            if (cuponDTO == null)
            {
                return BadRequest(new AuthResponseDto
                {
                    IsSuccess = false,
                    Message = "Datos inválidos"
                });
            }

            // Buscar el cupón existente por ID
            var cuponExistente = await _db.Cupones.FindAsync(id);

            if (cuponExistente == null)
            {
                return NotFound(new AuthResponseDto
                {
                    IsSuccess = false,
                    Message = "No se encontró el cupón para actualizar"
                });
            }

            // Asignar los valores del DTO al cupón existente
            cuponExistente.FechaCreacion = cuponDTO.FechaCreacion;
            cuponExistente.FechaExpiracion = cuponDTO.FechaExpiracion;
            cuponExistente.Codigo = cuponDTO.Codigo;
            cuponExistente.Tipo = cuponDTO.Tipo;
            cuponExistente.Paquete = cuponDTO.Paquete;
            cuponExistente.Cantidad = cuponDTO.Cantidad;
            cuponExistente.Valor = cuponDTO.Valor;
            cuponExistente.Usos = cuponDTO.Usos;
            cuponExistente.MontoMaximo = cuponDTO.MontoMaximo;
            cuponExistente.MontoMinimo = cuponDTO.MontoMinimo;
            cuponExistente.Activo = cuponDTO.Activo;

            // Si es necesario, también puedes asignar otros campos como IdUsuario o IdReceta si el DTO lo tiene.
            // Si no, asegúrate de que esos campos se mantengan igual en el modelo.

            _db.Cupones.Update(cuponExistente);
            await _db.SaveChangesAsync();

            return Ok(cuponExistente);
        }


        [HttpGet("obtener-cupon/{id}")]
        public async Task<ActionResult<Cupones>> ObtenerCupon(int id)
        {
            var cupon = await _db.Cupones.FindAsync(id);

            if (cupon == null)
            {
                return NotFound(new AuthResponseDto
                {
                    IsSuccess = false,
                    Message = "No se encontró el cupón"
                });
            }

            return Ok(cupon);
        }

        [HttpGet("obtener-todos-los-cupones")]
        public async Task<ActionResult<IEnumerable<Cupones>>> ObtenerTodosLosCupones()
        {
            var cupones = await _db.Cupones.ToListAsync();

            if (cupones == null || !cupones.Any())
            {
                return NotFound(new AuthResponseDto
                {
                    IsSuccess = false,
                    Message = "No se encontraron cupones"
                });
            }

            return Ok(cupones);
        }

        [HttpPost("validar-cupon")]
        public async Task<IActionResult> ValidarCupon([FromBody] string codigoCupon)
        {
            if (string.IsNullOrEmpty(codigoCupon))
            {
                return BadRequest(new AuthResponseDto
                {
                    IsSuccess = false,
                    Message = "Código de cupón inválido"
                });
            }

            var cupon = await _db.Cupones.FirstOrDefaultAsync(c => c.Codigo == codigoCupon);

            if (cupon == null)
            {
                return NotFound(new AuthResponseDto
                {
                    IsSuccess = false,
                    Message = "Cupón no encontrado"
                });
            }

            if (!cupon.Activo || cupon.FechaExpiracion < DateTime.Now || cupon.Usos <= 0)
            {
                return BadRequest(new AuthResponseDto
                {
                    IsSuccess = false,
                    Message = "Cupón no válido o sin usos disponibles"
                });

            }

            return Ok(new AuthResponseDto
            {
                IsSuccess = true,
                Message = "Cupón válido",
            });
        }

    }
}
