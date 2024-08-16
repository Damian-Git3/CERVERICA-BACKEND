using CERVERICA.Data;
using CERVERICA.DTO.Usuarios;
using CERVERICA.Dtos;
using CERVERICA.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace CERVERICA.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UsuariosController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _db;
        public UsuariosController(UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager,
        ApplicationDbContext context
        )
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _db = context;

        }

        [HttpGet]
        public async Task<ActionResult<UsuarioDTO>> obtenerUsuarios()
        {
            var users = await _userManager.Users.ToListAsync();

            var usuariosDto = new List<UsuarioDTO>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);

                usuariosDto.Add(new UsuarioDTO
                {
                    Id = user.Id,
                    Nombre = user.FullName,
                    Correo = user.Email,
                    Rol = roles.FirstOrDefault(),
                    Activo = user.Activo
                });
            }

            return Ok(usuariosDto);
        }


        [HttpPost("agregar")]
        public async Task<ActionResult<UsuarioDTO>> agregarUsuario([FromBody] CrearUsuarioDTO nuevoUsuario)
        {
            var user = new ApplicationUser
            {
                UserName = nuevoUsuario.Correo,
                Email = nuevoUsuario.Correo,
                FullName = nuevoUsuario.Nombre,
                Activo = true
            };

            var result = await _userManager.CreateAsync(user, nuevoUsuario.Password);

            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            var roleResult = await _userManager.AddToRoleAsync(user, nuevoUsuario.Rol);
            if (!roleResult.Succeeded)
            {
                return BadRequest(roleResult.Errors);
            }

            var usuarioDTO = new UsuarioDTO
            {
                Id = user.Id,
                Nombre = user.FullName,
                Correo = user.Email,
                Rol = nuevoUsuario.Rol,
                Activo = user.Activo
            };

            return CreatedAtAction(nameof(obtenerUsuarios), new { id = usuarioDTO.Id }, usuarioDTO);
        }

        [HttpPut("editar/{id}")]
        public async Task<ActionResult> editarUsuario(string id, [FromBody] EditarUsuarioDTO usuarioEditado)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                return NotFound(new[] { new { code = "UserNotFound", description = "El usuario no fue encontrado" } });
            }

            // Verificar si el rol al que se quiere cambiar es "Administrador"
            var currentRoles = await _userManager.GetRolesAsync(user);
            if (currentRoles.Contains("Administrador") && usuarioEditado.Rol != "Administrador")
            {
                var adminUsers = await _userManager.GetUsersInRoleAsync("Administrador");

                // Verificar si el usuario es el único Administrador
                if (adminUsers.Count == 1 && adminUsers.First().Id == user.Id)
                {
                    return BadRequest(new[] { new { code = "LastAdmin", description = "No se puede cambiar el rol del único Administrador restante" } });
                }
            }

            user.FullName = usuarioEditado.Nombre;
            user.Email = usuarioEditado.Correo;
            user.UserName = usuarioEditado.Correo;

            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                return BadRequest(new[] { new { code = "UserUpdateFailed", description = "La actualización del usuario falló", errors = result.Errors } });
            }

            // Actualizar la contraseña si se proporciona
            if (!string.IsNullOrEmpty(usuarioEditado.Password))
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var passwordResult = await _userManager.ResetPasswordAsync(user, token, usuarioEditado.Password);

                if (!passwordResult.Succeeded)
                {
                    return BadRequest(new[] { new { code = "PasswordUpdateFailed", description = "La actualización de la contraseña falló", errors = passwordResult.Errors } });
                }
            }

            // Actualizar el rol del usuario
            if (currentRoles.Any())
            {
                var removeRolesResult = await _userManager.RemoveFromRolesAsync(user, currentRoles);
                if (!removeRolesResult.Succeeded)
                {
                    return BadRequest(new[] { new { code = "RoleRemovalFailed", description = "La eliminación de roles falló", errors = removeRolesResult.Errors } });
                }
            }

            var roleResult = await _userManager.AddToRoleAsync(user, usuarioEditado.Rol);
            if (!roleResult.Succeeded)
            {
                return BadRequest(new[] { new { code = "RoleUpdateFailed", description = "La actualización del rol falló", errors = roleResult.Errors } });
            }

            return NoContent();
        }


        [HttpPut("activar/{id}")]
        public async Task<ActionResult> activarUsuario(string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            if (user.Activo)
            {
                return BadRequest("El usuario ya está activo");
            }

            user.Activo = true;

            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            return NoContent();
        }

        [HttpPut("desactivar/{id}")]
        public async Task<ActionResult> desactivarUsuario(string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            if (!user.Activo)
            {
                return BadRequest("El usuario ya está inactivo");
            }

            user.Activo = false;

            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            return NoContent();
        }



        [HttpDelete("eliminar/{id}")]
        public async Task<ActionResult> eliminarUsuario(string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            var result = await _userManager.DeleteAsync(user);

            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            return NoContent();
        }
    }
}
