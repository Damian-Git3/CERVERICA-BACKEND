using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using CERVERICA.Models;
using CERVERICA.Data;
using CERVERICA.Dtos;
using Microsoft.AspNetCore.Authorization;

namespace CERVERICA.Controllers
{
    [Authorize]
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
            RecetasDto[] recetas = await _context.Recetas
                .Select(r => new RecetasDto
                {
                    Id = r.Id,
                    LitrosEstimados = r.LitrosEstimados,
                    PrecioLitro = r.PrecioLitro,
                    Descripcion = r.Descripcion,
                    Nombre = r.Nombre,
                    CostoProduccion = r.CostoProduccion,
                    Imagen = r.Imagen,
                    Activo = r.Activo
                })
                .ToArrayAsync();

            return Ok(recetas);
        }

        // GET: api/recetas/5
        [HttpGet("{id}")]
        public async Task<ActionResult<RecetaDetallesDto>> GetReceta(int id)
        {
            var receta = await _context.Recetas
                .Include(r => r.IngredientesReceta)
                    .ThenInclude(ir => ir.Insumo)
                .Include(r => r.PasosReceta)
                .Where(r => r.Id == id)
                .Select(r => new RecetaDetallesDto
                {
                    Id = r.Id,
                    LitrosEstimados = r.LitrosEstimados,
                    PrecioLitro = r.PrecioLitro,
                    Descripcion = r.Descripcion,
                    Nombre = r.Nombre,
                    CostoProduccion = r.CostoProduccion,
                    Imagen = r.Imagen,
                    Activo = r.Activo,
                    IngredientesReceta = r.IngredientesReceta.Select(ir => new IngredienteRecetaDto
                    {
                        IdInsumo = ir.IdInsumo,
                        Cantidad = ir.Cantidad,
                        NombreInsumo = ir.Insumo.Nombre,
                        UnidadMedida = ir.Insumo.UnidadMedida
                    }).ToList(),
                    PasosReceta = r.PasosReceta.Select(pr => new PasosRecetaDto
                    {
                        Orden = pr.Orden,
                        Descripcion = pr.Descripcion,
                        Tiempo = pr.Tiempo
                    }).OrderBy(pr => pr.Orden).ToList()
                })
                .FirstOrDefaultAsync();

            if (receta == null)
            {
                return NotFound(new { message = "Receta no existe." });
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
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateReceta(int id, RecetaUpdateDto recetaDto)
        {
            if (recetaDto == null)
            {
                return BadRequest(new { message = "Datos de la receta son requeridos." });
            }

            var receta = await _context.Recetas
                .Include(r => r.IngredientesReceta)
                .FirstOrDefaultAsync(r => r.Id == id);

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

        [HttpPut("{id}/pasos")]
        public async Task<ActionResult> EditPasosInReceta(int id, List<PasosRecetaDto> pasosDto)
        {
            var receta = await _context.Recetas
                .Include(r => r.PasosReceta)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (receta == null)
            {
                return NotFound(new { message = "Receta no existe." });
            }

            // Eliminar pasos existentes que no están en la lista de pasosDto
            var pasosExistentes = receta.PasosReceta.ToList();
            foreach (var pasoExistente in pasosExistentes)
            {
                if (!pasosDto.Any(p => p.Orden == pasoExistente.Orden))
                {
                    _context.PasosRecetas.Remove(pasoExistente);
                }
            }

            // Actualizar o agregar pasos
            foreach (var pasoDto in pasosDto)
            {
                var pasoExistente = pasosExistentes.FirstOrDefault(p => p.Orden == pasoDto.Orden);
                if (pasoExistente != null)
                {
                    // Actualizar paso existente
                    pasoExistente.Descripcion = pasoDto.Descripcion;
                    pasoExistente.Tiempo = pasoDto.Tiempo;
                    _context.Entry(pasoExistente).State = EntityState.Modified;
                }
                else
                {
                    // Agregar nuevo paso
                    var nuevoPaso = new PasosReceta
                    {
                        IdReceta = receta.Id,
                        Orden = pasoDto.Orden,
                        Tiempo = pasoDto.Tiempo,
                        Descripcion = pasoDto.Descripcion
                    };
                    receta.PasosReceta.Add(nuevoPaso);
                }
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al actualizar los pasos.", error = ex.Message });
            }

            return Ok(new { message = "Pasos actualizados en la receta." });
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
        //NOTA: el precio por litro de la receta se actualiza solo si el costo por litro es mayor al valor actual
        //esto es para que el precio de venta sea siempre lo más estable posible y que solo se modifique si implica perdidas
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
                if (nuevoCostoLitro > receta.PrecioPaquete1)
                {
                    receta.PrecioPaquete1 = nuevoCostoLitro;
                }
                if (nuevoCostoLitro*6 > receta.PrecioPaquete6)
                {
                    receta.PrecioPaquete6 = nuevoCostoLitro*6;
                }
                if (nuevoCostoLitro*12 > receta.PrecioPaquete12)
                {
                    receta.PrecioPaquete12 = nuevoCostoLitro*12;
                }
                if (nuevoCostoLitro*24 > receta.PrecioPaquete24)
                {
                    receta.PrecioPaquete24 = nuevoCostoLitro*24;
                }


                receta.CostoProduccion = costoTotal;

                _context.Entry(receta).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }
        }

        [AllowAnonymous]
        [HttpGet("obtener-recetas-carousel")] // api/Usuario/1
        public async Task<IActionResult> getRecetasCarousel()
        {
            var productosCarousel = await _context.Recetas
                .Select(receta => new
                {
                    id = receta.Id,
                    nombre = receta.Nombre,
                    especificaciones = receta.Especificaciones,
                    imagen = receta.Imagen,
                    rutaFondo = receta.RutaFondo,
                })
                .ToListAsync();

            return Ok(productosCarousel);
        }

        [AllowAnonymous]
        [HttpGet("obtener-recetas-landing")] // api/Usuario/1
        public async Task<IActionResult> getRecetasLanding()
        {
            var productos = await _context.Recetas.Select(receta => new
            {
                id = receta.Id,
                nombre = receta.Nombre,
                especificaciones = receta.Especificaciones,
                precioPaquete1 = receta.PrecioPaquete1,
                precioPaquete6 = receta.PrecioPaquete6,
                precioPaquete12 = receta.PrecioPaquete12,
                precioPaquete24 = receta.PrecioPaquete24,
                fechaRegistrado = receta.FechaRegistrado,
                imagen = receta.Imagen,
                rutaFondo = receta.RutaFondo,
                cantidadEnStock = 200
                /* cantidadEnStock = _context.Stocks
                         .Where(stock => stock.IdReceta == receta.Id)
                         .Sum(stock => stock.Cantidad)*/
            }).ToListAsync();

            return Ok(productos);
        }

        [HttpPut("{id}/ActualizarPrecios")]
        public async Task<IActionResult> ActualizarPrecios(int id, [FromBody] UpdateRecetaPrecioDto dto)
        {
            var receta = await _context.Recetas.FindAsync(id);

            if (receta == null)
            {
                return NotFound($"No se encontró la receta con ID {id}");
            }

            // Validar los precios ingresados
            if ((dto.PrecioLitro.HasValue && dto.PrecioLitro.Value < receta.CostoProduccion/receta.LitrosEstimados) ||
                (dto.PrecioPaquete1.HasValue && dto.PrecioPaquete1.Value < receta.CostoProduccion / receta.LitrosEstimados) ||
                (dto.PrecioPaquete6.HasValue && dto.PrecioPaquete6.Value < 6 * receta.CostoProduccion / receta.LitrosEstimados) ||
                (dto.PrecioPaquete12.HasValue && dto.PrecioPaquete12.Value < 12 * receta.CostoProduccion / receta.LitrosEstimados) ||
                (dto.PrecioPaquete24.HasValue && dto.PrecioPaquete24.Value < 24 * receta.CostoProduccion / receta.LitrosEstimados))
            {
                return BadRequest("Uno o más precios son menores al costo de producción correspondiente.");
            }

            // Actualizar los precios si se proporcionaron
            if (dto.PrecioLitro.HasValue)
            {
                receta.PrecioLitro = dto.PrecioLitro.Value;
            }
            if (dto.PrecioPaquete1.HasValue)
            {
                receta.PrecioPaquete1 = dto.PrecioPaquete1.Value;
            }
            if (dto.PrecioPaquete6.HasValue)
            {
                receta.PrecioPaquete6 = dto.PrecioPaquete6.Value;
            }
            if (dto.PrecioPaquete12.HasValue)
            {
                receta.PrecioPaquete12 = dto.PrecioPaquete12.Value;
            }
            if (dto.PrecioPaquete24.HasValue)
            {
                receta.PrecioPaquete24 = dto.PrecioPaquete24.Value;
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Recetas.Any(r => r.Id == id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        [HttpGet("{id}/ConStock")]
        public async Task<ActionResult<RecetaConStockDto>> GetRecetaConStock(int id)
        {
            var receta = await _context.Recetas
                .Where(r => r.Id == id)
                .Select(r => new RecetaConStockDto
                {
                    Id = r.Id,
                    Nombre = r.Nombre,
                    Descripcion = r.Descripcion,
                    LitrosEstimados = r.LitrosEstimados,
                    PrecioLitro = r.PrecioLitro,
                    PrecioPaquete1 = r.PrecioPaquete1,
                    PrecioPaquete6 = r.PrecioPaquete6,
                    PrecioPaquete12 = r.PrecioPaquete12,
                    PrecioPaquete24 = r.PrecioPaquete24,
                    CostoProduccion = r.CostoProduccion,
                    CantidadEnStock = _context.Stocks
                        .Where(s => s.IdReceta == r.Id)
                        .Sum(s => s.Cantidad)
                })
                .FirstOrDefaultAsync();

            if (receta == null)
            {
                return NotFound($"No se encontró la receta con ID {id}");
            }

            return Ok(receta);
        }
    }
}
