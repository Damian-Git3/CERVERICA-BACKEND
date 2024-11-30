using CERVERICA.Data;
using CERVERICA.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CERVERICA.Dtos;

namespace CERVERICA.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriasRecetaController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CategoriasRecetaController(ApplicationDbContext context)
        {
            _context = context;
        }


        // GET: api/CategoriasReceta
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CategoriaCerveza>>> GetCategoriasReceta()
        {
            return await _context.CategoriasCerveza.ToArrayAsync();
        }

        // GET: api/CategoriasReceta/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CategoriaCerveza>> GetCategoriaCerveza(int id)
        {
            var categoriaCerveza = await _context.CategoriasCerveza.FindAsync(id);

            if (categoriaCerveza == null)
            {
                return NotFound();
            }

            return categoriaCerveza;
        }

        // PUT: api/CategoriasReceta/5
        [HttpPut]
        public async Task<IActionResult> PutCategoriaCerveza(CategoriaCervezaPutDto categoriaCervezaDTO)
        {
            //get the category
            var categoriaCerveza = await _context.CategoriasCerveza.FindAsync(categoriaCervezaDTO.Id);

            if (categoriaCerveza==null)
            {
                return BadRequest();
            }

            //update the category
            categoriaCerveza.Nombre = categoriaCervezaDTO.Nombre;

            await _context.SaveChangesAsync();

            _context.Entry(categoriaCerveza).State = EntityState.Modified;

            return Ok(categoriaCerveza);
        }

        // POST: api/CategoriasReceta
        [HttpPost]
        public async Task<ActionResult<CategoriaCerveza>> PostCategoriaCerveza(CategoriaCervezaInsertDto categoriaCervezaDTO)
        {
            var categoriaCerveza = new CategoriaCerveza
            {
                Nombre = categoriaCervezaDTO.Nombre,
                Estatus = true
            };

            _context.CategoriasCerveza.Add(categoriaCerveza);
            await _context.SaveChangesAsync();

            return Ok(new { id = categoriaCerveza.Id, categoriaCerveza });
        }

        // DELETE: api/CategoriasReceta/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<CategoriaCerveza>> DeleteCategoriaCerveza(int id)
        {
            var categoriaCerveza = await _context.CategoriasCerveza.FindAsync(id);
            if (categoriaCerveza == null)
            {
                return NotFound();
            }

            _context.CategoriasCerveza.Remove(categoriaCerveza);
            await _context.SaveChangesAsync();

            return categoriaCerveza;
        }

        // desactivar categoria
        [HttpPut("desactivar/{id}")]
        public async Task<ActionResult<CategoriaCerveza>> DesactivarCategoriaCerveza(int id)
        {
            var categoriaCerveza = await _context.CategoriasCerveza.FindAsync(id);
            if (categoriaCerveza == null)
            {
                return NotFound();
            }

            categoriaCerveza.Estatus = false;
            await _context.SaveChangesAsync();

            return categoriaCerveza;
        }

        // activar categoria
        [HttpPut("activar/{id}")]
        public async Task<ActionResult<CategoriaCerveza>> ActivarCategoriaCerveza(int id)
        {
            var categoriaCerveza = await _context.CategoriasCerveza.FindAsync(id);
            if (categoriaCerveza == null)
            {
                return NotFound();
            }

            categoriaCerveza.Estatus = true;
            await _context.SaveChangesAsync();

            return categoriaCerveza;
        }

        //asignar categorias a receta
        [HttpPost("asignar")]
        public async Task<ActionResult> AsignarCategoriasReceta(AsignarCategoriasRecetaDto asignarCategoriasRecetaDto)
        {
            // Verifica si el objeto asignarCategoriasRecetaDto o su lista CategoriasReceta es null
            if (asignarCategoriasRecetaDto == null || asignarCategoriasRecetaDto.CategoriasReceta == null)
            {
                return BadRequest(new { message = "El objeto de asignación o la lista de categorías no puede ser nulo." });
            }

            var receta = await _context.Recetas.FindAsync(asignarCategoriasRecetaDto.IdReceta);
            if (receta == null)
            {
                return NotFound(new { message = "Receta no encontrada." });
            }

            foreach (var Categoria in asignarCategoriasRecetaDto.CategoriasReceta)
            {
                var categoria = await _context.CategoriasCerveza.FindAsync(Categoria.Id);
                if (categoria != null)
                {
                    var recetaCategoria = new RecetaCategoriaCerveza
                    {
                        Receta = receta,
                        IdReceta = receta.Id,
                        CategoriaCerveza = categoria,
                        IdCategoriaCerveza = categoria.Id
                    };

                    _context.RecetasCategoriasCerveza.Add(recetaCategoria);
                }
            }
            await _context.SaveChangesAsync();

            return Ok(new { message = "Categorías asignadas con éxito." });
        }

        //obtener categorias de receta
        [HttpGet("receta/{idReceta}")]
        public async Task<ActionResult> ObtenerCategoriasReceta(int idReceta)
        {
            var receta = await _context.Recetas.FindAsync(idReceta);
            if (receta == null)
            {
                return NotFound(new { message = "Receta no encontrada." });
            }

            var categorias = await _context.RecetasCategoriasCerveza
                .Where(x => x.IdReceta == idReceta)
                .Select(x => new
                {
                    x.CategoriaCerveza.Id,
                    x.CategoriaCerveza.Nombre
                })
                .ToListAsync();

            return Ok(categorias);
        }
    }
}
