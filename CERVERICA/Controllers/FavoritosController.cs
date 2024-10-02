using CERVERICA.Data;
using CERVERICA.Dtos;
using CERVERICA.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Security.Claims;

namespace CERVERICA.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class FavoritosController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;

        public FavoritosController(UserManager<ApplicationUser> userManager, ApplicationDbContext db)
        {
            _userManager = userManager;
            _db = db;
        }

        [HttpPost("agregar-favorito")]
        public async Task<ActionResult<FavoritosComprador>> agregarFavorito(AgregarFavoritoUsuarioDTO favoritoUsuarioAgregar)
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

            var favoritoUsuarioExistente = await _db.FavoritosComprador
                .FirstOrDefaultAsync(f => f.IdUsuario == user.Id && f.IdReceta == favoritoUsuarioAgregar.IdReceta);
            if(favoritoUsuarioExistente != null)
            {
                return BadRequest("La cerveza seleccionada ya es un favorito.");
            }

            var nuevoFavoritoUsuario = new FavoritosComprador();
            nuevoFavoritoUsuario.IdUsuario = user.Id;
            nuevoFavoritoUsuario.IdReceta = favoritoUsuarioAgregar.IdReceta;

            _db.FavoritosComprador.Add(nuevoFavoritoUsuario);
            await _db.SaveChangesAsync();

            //obtener el nombre de la cerveza 
            var receta = await _db.Recetas.FirstOrDefaultAsync(r => r.Id == favoritoUsuarioAgregar.IdReceta);

            Notificacion notificacion = new Notificacion
            {
                IdUsuario = user.Id,
                Fecha = DateTime.Now,
                Tipo = 2,
                //mostrar el mensaje de que guardo la cerveza en favoritos
                Mensaje = "Guardaste la cerveza " + receta.Nombre + " en favoritos",
                Visto = false
            };

            _db.Notificaciones.Add(notificacion);
            await _db.SaveChangesAsync();

            return Ok(nuevoFavoritoUsuario);
        }

        [HttpPost("eliminar-favorito")]
        public async Task<ActionResult<string>> EliminarFavorito(EliminarFavoritoUsuarioDTO favoritoUsuario)
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

            var favoritoUsuarioEliminar = await _db.FavoritosComprador.FirstOrDefaultAsync(f => f.IdUsuario == user.Id && f.IdReceta == favoritoUsuario.IdReceta);

            if (favoritoUsuarioEliminar == null)
            {
                return NotFound("Favorito no encontrado.");
            }

            _db.FavoritosComprador.Remove(favoritoUsuarioEliminar);

            await _db.SaveChangesAsync();

            //obtener el nombre de la cerveza 
            var receta = await _db.Recetas.FirstOrDefaultAsync(r => r.Id == favoritoUsuario.IdReceta);

            Notificacion notificacion = new Notificacion
            {
                IdUsuario = user.Id,
                Fecha = DateTime.Now,
                Tipo = 2,
                //mostrar el mensaje de que guardo la cerveza en favoritos
                Mensaje = "Quitaste la cerveza " + receta.Nombre + " de favoritos",
                Visto = false
            };

            _db.Notificaciones.Add(notificacion);
            await _db.SaveChangesAsync();

            return Ok(new { message = "Favorito eliminado exitosamente" });
        }

        [HttpGet("obtener-favoritos/{idUsuario}")]
        public async Task<ActionResult<FavoritoDto>> obtenerFavoritos(string idUsuario)
        {
            //buscar usuario
            var usuario = await _userManager.FindByIdAsync(idUsuario);
            if (usuario == null)
            {
                return NotFound(new AuthResponseDto
                {
                    IsSuccess = false,
                    Message = "Usuario no encontrado"
                });
            }

            var favoritos = await _db.FavoritosComprador.Where(f => f.IdUsuario == idUsuario)
                .Include(f => f.Receta).Select(f => new FavoritoDto
                {
                    Id = f.Id,
                    IdReceta = f.IdReceta,
                    IdUsuario = f.IdUsuario,
                    Nombre = f.Receta.Nombre,
                    Descripcion = f.Receta.Descripcion,
                    Imagen = f.Receta.Imagen,
                    RutaFondo = f.Receta.RutaFondo,
                    Especificaciones = f.Receta.Especificaciones
                }).ToListAsync();
                return Ok(favoritos);
        }

        [HttpGet("obtener-favoritos")]
        public async Task<ActionResult<FavoritosComprador>> obtenerFavoritos()

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

            var favoritos = await _db.FavoritosComprador
            .Where(f => f.IdUsuario == user.Id)
            .ToListAsync();

            return Ok(favoritos);
        }
    }
}
