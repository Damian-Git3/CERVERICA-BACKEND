using CERVERICA.Data;
using CERVERICA.DTO.Recetas;
using CERVERICA.Dtos;
using CERVERICA.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CERVERICA.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HistorialPreciosController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<HistorialPreciosController> _logger;

        /* CONSTRUCTOR */
        public HistorialPreciosController(ApplicationDbContext context, ILogger<HistorialPreciosController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpPost]
        public async Task<ActionResult<object>> CrearHistorial(HistorialPreciosInsert hp)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {

                var historial = new HistorialPrecios
                {
                    IdReceta = hp.IdReceta,
                    Paquete1 = hp.Paquete1,
                    Paquete6 = hp.Paquete6,
                    Paquete12 = hp.Paquete12,
                    Paquete24 = hp.Paquete24,
                    CostoProduccionUnidad = hp.CostoProduccionUnidad,
                    PrecioUnitarioMinimoMayoreo = hp.PrecioUnitarioMinimoMayoreo,
                    PrecioUnitarioBaseMayoreo = hp.PrecioUnitarioBaseMayoreo
                };

                _context.HistorialPrecios.Add(historial);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Historial Creado Correctamente", id = historial.Id });

            }
            catch (Exception ex)
            {
                // Log the exception (optional)
                _logger.LogError(ex, "Error Creando historial de precios");

                // Return a generic error response
                return StatusCode(500, new { message = "Ocurrió un error al crear el historial de precios.", details = ex.Message });
            }

        }

        [HttpGet("{id}")]
        public async Task<ActionResult<HistorialPrecioResponse>> GetHistorialPrecios(int id)
        {
            try
            {
                _logger.LogDebug("GetHistorialPrecios: {Id}", id);
                var historialEntity = await _context.HistorialPrecios.FindAsync(id);

                if (historialEntity == null)
                {
                    return NotFound();
                }

                var historial = new HistorialPrecioResponse(historialEntity);

                return Ok(historial);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error obteniendo el historial de precios");

                return StatusCode(500, new { message = "Ocurrio un error al obtener el historial de precio" });
            }
        }

        [HttpGet]
        [Route("ListarRecetas")]
        public async Task<ActionResult<List<RecetasViewDTO>>> ListarRecetas()
        {
            try
            {
                var recetas = await _context.Recetas
                    .Select(r => new RecetasViewDTO
                    {
                        Id = r.Id,
                        Nombre = r.Nombre,
                        Imagen = r.Imagen,
                        Activo = r.Activo
                    })
                    .ToListAsync();

                return Ok(recetas);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error obteniendo la lista de recetas");

                return StatusCode(500, new { message = "Ocurrió un error al obtener la lista de recetas" });
            }
        }
    }
}
