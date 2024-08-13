﻿using CERVERICA.Data;
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
        public async Task<ActionResult<FavoritoUsuario>> agregarFavorito(AgregarFavoritoUsuarioDTO favoritoUsuarioAgregar)
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

            var favoritoUsuarioExistente = await _db.FavoritosUsuarios
                .FirstOrDefaultAsync(f => f.IdUsuario == user.Id && f.IdReceta == favoritoUsuarioAgregar.IdReceta);
            if(favoritoUsuarioExistente != null)
            {
                return BadRequest("La cerveza seleccionada ya es un favorito.");
            }

            var nuevoFavoritoUsuario = new FavoritoUsuario();
            nuevoFavoritoUsuario.IdUsuario = user.Id;
            nuevoFavoritoUsuario.IdReceta = favoritoUsuarioAgregar.IdReceta;

            _db.FavoritosUsuarios.Add(nuevoFavoritoUsuario);
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

            var favoritoUsuarioEliminar = await _db.FavoritosUsuarios.FirstOrDefaultAsync(f => f.IdUsuario == user.Id && f.IdReceta == favoritoUsuario.IdReceta);

            if (favoritoUsuarioEliminar == null)
            {
                return NotFound("Favorito no encontrado.");
            }

            _db.FavoritosUsuarios.Remove(favoritoUsuarioEliminar);

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

            var favoritos = await _db.FavoritosUsuarios.Where(f => f.IdUsuario == idUsuario)
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
        public async Task<ActionResult<FavoritoUsuario>> obtenerFavoritos()

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

            var favoritos = await _db.FavoritosUsuarios
            .Where(f => f.IdUsuario == user.Id)
            .ToListAsync();

            return Ok(favoritos);
        }
    }
}
