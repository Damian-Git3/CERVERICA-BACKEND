using CERVERICA.Data;
using CERVERICA.DTO.Comentarios;
using CERVERICA.Dtos;
using CERVERICA.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CERVERICA.Controllers
{


    [Route("api/[controller]")]
    [ApiController]
    public class ComentariosController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;

        public ComentariosController(UserManager<ApplicationUser> userManager, ApplicationDbContext db)
        {
            _userManager = userManager;
            _db = db;
        }

        [HttpGet("obtener-comentarios/{idReceta}")]
        public async Task<ActionResult<Comentario>> obtenerComentarios(int idReceta)
        {
            var comentarios = await _db.Comentarios
            .Where(comentario => comentario.IdReceta == idReceta)
            .Select(comentario => new ComentarioDTO
            {
                Id = comentario.Id,
                Puntuacion = comentario.Puntuacion,
                TextoComentario = comentario.TextoComentario,
                NombreUsuario = comentario.Usuario.FullName,
                Fecha = comentario.Fecha,
                IdUsuario = comentario.IdUsuario,
                IdReceta = comentario.IdReceta
            })
            .OrderByDescending(comentario => comentario.Puntuacion)
            .OrderBy(comentario => comentario.Fecha)
            .ToListAsync();

            return Ok(comentarios);
        }

        [HttpPost("agregar-comentario")]
        public async Task<ActionResult<Comentario>> AgregarComentario(NuevoComentario nuevoComentario)
        {
            var usuario = await _userManager.GetUserAsync(User);

            if (usuario == null)
            {
                return NotFound(new AuthResponseDto
                {
                    IsSuccess = false,
                    Message = "Fallo al autenticar"
                });
            }

            var comentario = new Comentario
            {
                Puntuacion = nuevoComentario.Puntuacion,
                TextoComentario = nuevoComentario.TextoComentario,
                Fecha = DateTime.Now,
                IdReceta = nuevoComentario.IdReceta,
                IdUsuario = usuario.Id,
            };

            // Guardar en la base de datos
            _db.Comentarios.Add(comentario);
            await _db.SaveChangesAsync();

            // Obtener todos los comentarios de la receta
            var comentariosReceta = await _db.Comentarios
                .Where(c => c.IdReceta == nuevoComentario.IdReceta)
                .ToListAsync();

            // Calcular el promedio de las puntuaciones
            var totalPuntuaciones = comentariosReceta.Sum(c => c.Puntuacion);
            var promedioPuntuacion = totalPuntuaciones / comentariosReceta.Count;

            // Actualizar la puntuación de la receta
            var receta = await _db.Recetas.FindAsync(nuevoComentario.IdReceta);
            if (receta == null)
            {
                return NotFound("Receta no encontrada.");
            }

            receta.Puntuacion = promedioPuntuacion;

            // Guardar los cambios en la base de datos
            _db.Recetas.Update(receta);
            await _db.SaveChangesAsync();

            return Ok(promedioPuntuacion);
        }
    }
}
