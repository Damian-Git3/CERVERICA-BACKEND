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
    [Authorize]
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
                CategoriaComprador = cuponDTO.CategoriaComprador,
                Activo = cuponDTO.Activo
                // No se incluyen IdUsuario ni IdReceta en el DTO
            };

            await _db.Cupones.AddAsync(cupon);
            await _db.SaveChangesAsync();

            return Created("api/cupones/registrar-cupon", cupon);
        }


        [HttpPut("actualizar-cupon/{id}")]
        public async Task<ActionResult<Cupones>> ActualizarCupon(int id, [FromBody] Cupones cupon)
        {
            if (cupon == null)
            {
                return BadRequest(new AuthResponseDto
                {
                    IsSuccess = false,
                    Message = "Datos inválidos"
                });
            }

            // Buscar el cupon existente por ID
            var cuponExistente = await _db.Cupones.FindAsync(id);

            if (cuponExistente == null)
            {
                return NotFound(new AuthResponseDto
                {
                    IsSuccess = false,
                    Message = "No se encontró el cupón para actualizar"
                });
            }

            // Actualizar los campos necesarios
            cuponExistente.IdUsuario = cupon.IdUsuario;
            cuponExistente.IdReceta = cupon.IdReceta;
            cuponExistente.FechaCreacion = cupon.FechaCreacion;
            cuponExistente.FechaExpiracion = cupon.FechaExpiracion;
            cuponExistente.Codigo = cupon.Codigo;
            cuponExistente.Tipo = cupon.Tipo;
            cuponExistente.Paquete = cupon.Paquete;
            cuponExistente.Cantidad = cupon.Cantidad;
            cuponExistente.Valor = cupon.Valor;
            cuponExistente.Usos = cupon.Usos;
            cuponExistente.MontoMaximo = cupon.MontoMaximo;
            cuponExistente.CategoriaComprador = cupon.CategoriaComprador;
            cuponExistente.Activo = cupon.Activo;

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

    }
}
