using CERVERICA.Data;
using CERVERICA.DTO.HistorialPrecios;
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
            try
            {
                _logger.LogDebug("Iniciando la creación del historial de precios para la receta con Id: {IdReceta}", hp.IdReceta);

                var receta = await _context.Recetas.FindAsync(hp.IdReceta);

                if (receta == null)
                {
                    _logger.LogWarning("Receta no encontrada con Id: {IdReceta}", hp.IdReceta);
                    return NotFound(new { message = "Receta no encontrada" });
                }

                var historial = new HistorialPrecios
                {
                    IdReceta = hp.IdReceta,
                    Paquete1 = hp.Paquete1,
                    Paquete6 = hp.Paquete6,
                    Paquete12 = hp.Paquete12,
                    Paquete24 = hp.Paquete24,
                    CostoProduccionUnidad = 0,
                    PrecioUnitarioMinimoMayoreo = 0,
                    PrecioUnitarioBaseMayoreo = 0
                };

                _context.HistorialPrecios.Add(historial);

                receta.PrecioPaquete1 = (float)hp.Paquete1;
                receta.PrecioPaquete6 = (float)hp.Paquete6;
                receta.PrecioPaquete12 = (float)hp.Paquete12;
                receta.PrecioPaquete24 = (float)hp.Paquete24;

                _logger.LogDebug("Guardando cambios en la base de datos");
                await _context.SaveChangesAsync();

                _logger.LogInformation("Historial de precios creado correctamente con Id: {Id}", historial.Id);
                return Ok(new { message = "Historial Creado Correctamente", id = historial.Id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error Creando historial de precios");

                return StatusCode(500, new { message = "Ocurrió un error al crear el historial de precios.", details = ex.Message });
            }
        }


        [HttpGet]
        [Route("ListarRecetas")]
        public async Task<ActionResult<List<RecetasViewDTO>>> ListarRecetas()
        {
            try
            {

                var recetas = await _context.Recetas
                    .Select(static r => new RecetasViewDTO
                    {
                        Id = r.Id,
                        Nombre = r.Nombre,
                        Precio = (float)Math.Round(r.PrecioPaquete1.HasValue ? (float)r.PrecioPaquete1.Value : 0f, 2),
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

        [HttpGet]
        [Route("PreciosReceta")]
        public async Task<ActionResult<PreciosRecetaDTO>> GetPreciosReceta(int Id)
        {
            try
            {
                var receta = await _context.Recetas
                    .Where(r => r.Id == Id)
                    .Select(r => new PreciosRecetaDTO
                    {
                        Id = r.Id,
                        Nombre = r.Nombre,
                        PrecioLitro = (float)Math.Round((float)r.PrecioLitro, 2),
                        PrecioPaquet1 = (float)(r.PrecioPaquete1.HasValue ? Math.Round((float)r.PrecioPaquete1.Value, 2) : 0),
                        PrecioPaquete6 = (float)Math.Round((float)r.PrecioPaquete6, 2),
                        PrecioPaquete12 = (float)Math.Round((float)r.PrecioPaquete12, 2),
                        PrecioPaquete24 = (float)Math.Round((float)r.PrecioPaquete24, 2),
                        Imagen = r.Imagen,
                        Estatus = r.Activo
                    }).FirstOrDefaultAsync();


                return Ok(receta);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error obteniendo la lista de recetas");

                return StatusCode(500, new { message = "Ocurrió un error al obtener la lista de recetas" });
            }
        }

        [HttpGet]
        [Route("ListaHistorialPrecios")]
        public async Task<ActionResult<List<PreciosReceta>>> GetListaHistorialPrecios(int IdReceta)
        {
            try
            {
                var historial = await _context.HistorialPrecios
                    .Where(r => r.IdReceta == IdReceta)
                    .OrderBy(r => r.Id) // Ordenar por el campo "Id"
                    .Select(r => new PreciosReceta
                    {
                        Fecha = r.Fecha,
                        PrecioPaquete1 = (float)Math.Round((float)r.Paquete1, 2),
                        PrecioPaquete6 = (float)Math.Round((float)r.Paquete6, 2),
                        PrecioPaquete12 = (float)Math.Round((float)r.Paquete12, 2),
                        PrecioPaquete24 = (float)Math.Round((float)r.Paquete24, 2),
                    }).ToListAsync();


                return Ok(historial);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error obteniendo la lista de precios");

                return StatusCode(500, new { message = "Ocurrió un error al obtener la lista de precios" });
            }
        }
    }
}
