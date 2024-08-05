using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using CERVERICA.Models;
using CERVERICA.Data;
using CERVERICA.Dtos;

namespace CERVERICA.Controllers
{
    //[Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class RecetaController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public RecetaController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/recetas
        [HttpGet]
        public async Task<ActionResult<IEnumerable<RecetasDto>>> GetRecetas()
        {
            return Ok(await _context.Recetas.ToListAsync());
        }

        // GET: api/recetas/5
        [HttpGet("{id}")]
        public async Task<ActionResult<RecetasDto>> GetReceta(int id)
        {
            var receta = await _context.Recetas.FindAsync(id);

            if (receta == null)
            {
                return NotFound(new {message="Receta no existe."});
            }

            return Ok(receta);
        }

        // POST: api/recetas
        [HttpPost]
        public async Task<ActionResult<Receta>> PostReceta(RecetaInsertDto recetaDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var receta = new Receta
            {
                LitrosEstimados = recetaDto.LitrosEstimados,
                PrecioLitro = 0, 
                Descripcion = recetaDto.Descripcion,
                Nombre = recetaDto.Nombre,
                CostoProduccion = 0,
                Imagen = recetaDto.Imagen,
                Activo = true,
                IngredientesReceta = new List<IngredienteReceta>()
            };

            foreach (var ingredienteDto in recetaDto.IngredientesReceta)
            {
                var insumo = await _context.Insumos.FindAsync(ingredienteDto.IdInsumo);
                if (insumo == null)
                {
                    return NotFound(new { message = $"Insumo con ID {ingredienteDto.IdInsumo} no encontrado." });
                }

                var ingredienteReceta = new IngredienteReceta
                {
                    IdInsumo = ingredienteDto.IdInsumo,
                    Cantidad = ingredienteDto.Cantidad,
                    Receta = receta
                };

                receta.IngredientesReceta.Add(ingredienteReceta);
            }

            _context.Recetas.Add(receta);
            await _context.SaveChangesAsync();

            await CalcularCostoProduccion(receta.Id);

            return Ok(new {message="Receta insertada.", id = receta.Id });
        }

        // PUT: api/recetas/5
        [HttpPut]
        public async Task<IActionResult> UpdateReceta(RecetaUpdateDto recetaDto)
        {
            if (recetaDto == null || recetaDto.Id == 0)
            {
                return BadRequest(new { message = "Datos de la receta son requeridos." });
            }

            var receta = await _context.Recetas
                .Include(r => r.IngredientesReceta)
                .FirstOrDefaultAsync(r => r.Id == recetaDto.Id);

            if (receta == null)
            {
                return NotFound(new { message = "Receta no encontrada." });
            }

            // Actualizar propiedades de la receta
            receta.LitrosEstimados = recetaDto.LitrosEstimados;
            receta.Descripcion = recetaDto.Descripcion;
            receta.Nombre = recetaDto.Nombre;
            receta.CostoProduccion = recetaDto.CostoProduccion;
            receta.Imagen = recetaDto.Imagen;
            receta.Activo = recetaDto.Activo;

            // Actualizar ingredientes
            var ingredientesExistentes = receta.IngredientesReceta.ToList();

            foreach (var ingredienteDto in recetaDto.IngredientesReceta)
            {
                var ingredienteExistente = ingredientesExistentes
                    .FirstOrDefault(i => i.IdInsumo == ingredienteDto.IdInsumo);

                if (ingredienteExistente != null)
                {
                    // Actualizar ingrediente existente
                    ingredienteExistente.Cantidad = ingredienteDto.Cantidad;
                    _context.Entry(ingredienteExistente).State = EntityState.Modified;
                    ingredientesExistentes.Remove(ingredienteExistente);
                }
                else
                {
                    // Agregar nuevo ingrediente
                    var ingredienteNuevo = new IngredienteReceta
                    {
                        IdReceta = receta.Id,
                        IdInsumo = ingredienteDto.IdInsumo,
                        Cantidad = ingredienteDto.Cantidad
                    };
                    _context.IngredientesReceta.Add(ingredienteNuevo);
                }
            }

            // Eliminar ingredientes que ya no están en la lista
            foreach (var ingredienteEliminar in ingredientesExistentes)
            {
                _context.IngredientesReceta.Remove(ingredienteEliminar);
            }

            try
            {
                await _context.SaveChangesAsync();
                await CalcularCostoProduccion(receta.Id);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RecetaExists(recetaDto.Id))
                {
                    return NotFound(new { message = "Receta no existe." });
                }
                else
                {
                    throw;
                }
            }


            return Ok(new { message = "Receta actualizada exitosamente." });
        }

        //activar receta
        [HttpPost("activar/{id}")]
        public async Task<IActionResult> ActivarReceta(int id)
        {
            var receta = await _context.Recetas.FindAsync(id);

            if (receta == null)
            {
                return NotFound(new { message = "Receta no existe." });
            }

            receta.Activo = true;

            _context.Entry(receta).State = EntityState.Modified;

            await _context.SaveChangesAsync();

            return Ok(new { message = "Receta activada." });
        }

        //Desactivar receta
        [HttpPost("desactivar/{id}")]
        public async Task<IActionResult> DesactivarReceta(int id)
        {
            var receta = await _context.Recetas.FindAsync(id);

            if (receta == null)
            {
                return NotFound(new { message = "Receta no existe." });
            }

            receta.Activo = false;

            _context.Entry(receta).State = EntityState.Modified;

            await _context.SaveChangesAsync();

            return Ok(new { message = "Receta desactivada." });
        }

        // DELETE: api/recetas/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteReceta(int id)
        {
            var receta = await _context.Recetas.FindAsync(id);

            if (receta == null)
            {
                return NotFound(new { message = "Receta no existe." });
            }

            _context.Recetas.Remove(receta);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Receta eliminada." });
        }

        private bool RecetaExists(int id)
        {
            return _context.Recetas.Any(e => e.Id == id);
        }

        //calcular costo de produccion
        private async Task CalcularCostoProduccion(int idReceta)
        {
            var receta = await _context.Recetas
                .Include(r => r.IngredientesReceta)
                    .ThenInclude(ir => ir.Insumo)
                .FirstOrDefaultAsync(r => r.Id == idReceta);

            if (receta != null)
            {
                float costoTotal = 0;

                foreach (var ingrediente in receta.IngredientesReceta)
                {
                    var costoUnitario = ingrediente.Insumo.CostoUnitario;

                    costoTotal += ingrediente.Cantidad * costoUnitario;
                }
                var nuevoCostoLitro = costoTotal / receta.LitrosEstimados;

                if(nuevoCostoLitro > receta.PrecioLitro)
                {
                    receta.PrecioLitro = nuevoCostoLitro;
                }
                receta.CostoProduccion = costoTotal;

                _context.Entry(receta).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }
        }
    }
}
