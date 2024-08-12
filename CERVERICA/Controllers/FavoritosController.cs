using CERVERICA.Data;
using CERVERICA.Dtos;
using CERVERICA.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CERVERICA.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class FavoritosController : ControllerBase
    {
        private readonly ApplicationDbContext _db;

        public FavoritosController(ApplicationDbContext db)
        {
            _db = db;
        }

        [HttpPost("agregar-favorito")]
        public async Task<ActionResult<FavoritoUsuario>> agregarFavorito(FavoritoUsuario favoritoUsuario)
        {
            _db.FavoritosUsuarios.Add(favoritoUsuario);
            await _db.SaveChangesAsync();

            return Ok(favoritoUsuario);
        }

        [HttpPost("eliminar-favorito")]
        public async Task<ActionResult<string>> EliminarFavorito(FavoritoUsuario favoritoUsuario)
        {
            var favoritoUsuarioEliminar = await _db.FavoritosUsuarios
                .FirstOrDefaultAsync(f => f.IdUsuario == favoritoUsuario.IdUsuario && f.IdReceta == favoritoUsuario.IdReceta);

            if (favoritoUsuarioEliminar == null)
            {
                return NotFound("Favorito no encontrado.");
            }

            _db.FavoritosUsuarios.Remove(favoritoUsuarioEliminar);

            await _db.SaveChangesAsync();

            return Ok(new { message = "Favorito eliminado exitosamente." });
        }

        [HttpGet("obtener-favoritos/{idUsuario}")]
        public async Task<ActionResult<FavoritoDto>> obtenerFavoritos(string idUsuario)
        {
            var favoritos = await _db.FavoritosUsuarios
            .Where(f => f.IdUsuario == idUsuario)
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
    }
}
