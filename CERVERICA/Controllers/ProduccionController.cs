using CERVERICA.Data;
using CERVERICA.Dtos;
using CERVERICA.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;


namespace CERVERICA.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize(Roles = "Vendedor")]
    public class ProduccionController(ApplicationDbContext context, ILogger<ProduccionController> logger) : ControllerBase
    {
        private readonly ApplicationDbContext _context = context;
        private readonly ILogger<ProduccionController> _logger = logger;

        // GET: api/Produccion
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProduccionesDto>>> GetProducciones()
        {
            var producciones = await _context.Producciones
                .Where(p => p.Estatus != 4)
                .Include(p => p.Receta)
                    .ThenInclude(r => r.IngredientesReceta!)
                    .ThenInclude(ir => ir.Insumo)
                .Select(p => new ProduccionesDto
                {
                    Id = p.Id,
                    FechaProduccion = p.FechaProduccion,
                    Mensaje = p.Mensaje,
                    Estatus = p.Estatus,
                    NumeroTandas = p.NumeroTandas,
                    IdReceta = p.IdReceta,
                    Receta = new RecetasDto
                    {
                        Id = p.Receta.Id,
                        Nombre = p.Receta.Nombre,
                        Descripcion = p.Receta.Descripcion,
                        LitrosEstimados = p.Receta.LitrosEstimados,
                        PrecioLitro = p.Receta.PrecioLitro,
                        CostoProduccion = p.Receta.CostoProduccion,
                        Imagen = p.Receta.Imagen,
                        RutaFondo = p.Receta.RutaFondo,
                        Activo = p.Receta.Activo
                    },
                    NombreReceta = p.Receta.Nombre,
                    FechaSolicitud = p.FechaSolicitud,
                    Paso = p.Paso,
                    DescripcionPaso = _context.PasosRecetas
                        .Where(pp => pp.IdReceta == p.IdReceta && pp.Orden == p.Paso)
                        .Select(pp => pp.Descripcion)
                        .FirstOrDefault() ?? "Sin descripción",
                    IdUsuarioSolicitud = p.IdUsuarioSolicitud,
                    IdUsuario = p.IdUsuarioProduccion,
                    NombreUsuario = _context.Users
                        .Where(u => u.Id == p.IdUsuarioProduccion)
                        .Select(u => u.FullName)
                        .FirstOrDefault() ?? "Sin nombre"
                })
                .ToListAsync();

            return Ok(producciones);
        }

        // GET: api/Produccion/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ProduccionDto>> GetProduccion(int id)
        {
            var produccion = await _context.Producciones
                .Include(p => p.Receta)
                    .ThenInclude(r => r.IngredientesReceta!)
                        .ThenInclude(ir => ir.Insumo)
                .Include(p => p.ProduccionLoteInsumos)
                .Where(p => p.Id == id)
                .Select(p => new ProduccionDto
                {
                    Id = p.Id,
                    FechaProduccion = p.FechaProduccion,
                    FechaProximoPaso = p.FechaProximoPaso,
                    Mensaje = p.Mensaje,
                    Estatus = p.Estatus,
                    NumeroTandas = p.NumeroTandas,
                    IdReceta = p.IdReceta,
                    FechaSolicitud = p.FechaSolicitud,
                    IdUsuarioSolicitud = p.IdUsuarioSolicitud,
                    IdUsuarioProduccion = p.IdUsuarioProduccion,
                    PasoActual = p.Paso,
                    DescripcionPasoActual = _context.PasosRecetas
                        .Where(pp => pp.IdReceta == p.IdReceta && pp.Orden == p.Paso)
                        .Select(pp => pp.Descripcion)
                        .FirstOrDefault() ?? "Sin descripción",
                    PasosReceta = _context.PasosRecetas
                        .Where(pp => pp.IdReceta == p.IdReceta)
                        .Select(pp => new PasosRecetaDto
                        {
                            Orden = pp.Orden,
                            Descripcion = pp.Descripcion,
                            Tiempo = pp.Tiempo
                        })
                        .ToList(),
                    Receta = new RecetaProduccionDto
                    {
                        Id = p.Receta.Id,
                        Nombre = p.Receta.Nombre,
                        IngredientesReceta = p.Receta.IngredientesReceta.Select(ir => new IngredienteRecetaDetalleDto
                        {
                            IdInsumo = ir.IdInsumo,
                            NombreInsumo = ir.Insumo.Nombre,
                            Cantidad = ir.Cantidad,
                            UnidadMedida = ir.Insumo.UnidadMedida,
                            CostoUnitario = ir.Insumo.CostoUnitario
                        }).ToList()
                    },
                    ProduccionLoteInsumos = p.ProduccionLoteInsumos.Select(pli => new ProduccionLoteInsumoDto
                    {
                        Id = pli.Id,
                        Cantidad = pli.Cantidad,
                        IdInsumo = pli.LoteInsumo.IdInsumo,
                        IdLoteInsumo = pli.IdLoteInsumo,
                        NombreInsumo = pli.LoteInsumo.Insumo.Nombre,
                        UnidadMedida = pli.LoteInsumo.Insumo.UnidadMedida
                    }).ToList()
                })
                .FirstOrDefaultAsync();

            if (produccion == null)
            {
                return NotFound();
            }

            return Ok(produccion);
        }

        //Metodos del vendedor

        // POST: api/Produccion
        [HttpPost]
        public async Task<IActionResult> SolicitarProduccion([FromBody] SolicitarProduccionDto solicitudDto)
        {
            _logger.LogInformation("Iniciando solicitud de producción para {IdReceta}", solicitudDto.IdReceta);

            //si el numero de tandas es menor a 1
            if (solicitudDto.NumeroTandas < 1)
            {
                return BadRequest(new { message = "El número de tandas debe ser mayor a 0." });
            }

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var receta = await _context.Recetas
                    .Include(r => r.IngredientesReceta)
                        .ThenInclude(ir => ir.Insumo)
                    .Where(r => r.Id == solicitudDto.IdReceta)
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
                            Id = ir.IdInsumo,
                            Cantidad = ir.Cantidad,
                            Nombre = ir.Insumo.Nombre,
                            UnidadMedida = ir.Insumo.UnidadMedida
                        }).ToList()
                    })
                    .FirstOrDefaultAsync();

                if (receta == null)
                {
                    return NotFound("Receta no encontrada.");
                }

                var usuarioProduccion = await _context.Users.FindAsync(solicitudDto.IdUsuario);
                if (usuarioProduccion == null)
                {
                    return NotFound(new { message = "Usuario no existe. Escoja otro usuario de Producción." });
                }

                //calcular el numero de botella de 355 ml que se pueden llenar y si no hay stock de botellas rechazar la solicitud
                var medidaEnvase = 355;
                var litrosEstimados = receta.LitrosEstimados * solicitudDto.NumeroTandas;
                var mililitrosEstimados = litrosEstimados * 1000;

                var botellasNecesarias = Math.Ceiling(mililitrosEstimados / medidaEnvase);

                //consultar el stock de botellas con Id = 2 en la tabla de insumos y lotes de insumos
                var stockBotellas = await _context.LotesInsumos
                    .Where(li => li.IdInsumo == 2 && li.FechaCaducidad > DateTime.Today)
                    .SumAsync(li => li.Cantidad);


                if (botellasNecesarias > stockBotellas)
                {
                    return BadRequest(new { message = "No hay suficientes botellas de 355 ml en stock para la producción." });
                }

                //consultar todas todas las producciones en estatus 1, 2 y 3
                var produccionesEnProceso = await _context.Producciones
                    .Where(p => p.Estatus == 1 || p.Estatus == 2 || p.Estatus == 3)
                    .ToListAsync();

                var sumatoriaMililitrosEsperados = 0.0;

                //recorrer las producciones en proceso y sumar los litros esperados de cada una de ellas
                foreach (var prod in produccionesEnProceso)
                {

                    //hacer una consulta para obtener los litros estimados de la receta de cada produccion
                    var recetaProdLitrosEstimados = await _context.Recetas
                        .Where(r => r.Id == prod.IdReceta)
                        .Select(r => new
                        {
                            LitrosEstimados = r.LitrosEstimados
                        })
                        .FirstOrDefaultAsync();

                    sumatoriaMililitrosEsperados += Math.Ceiling((float)recetaProdLitrosEstimados.LitrosEstimados * prod.NumeroTandas * 1000);
                }

                var sumatoriaMililitrosEsperadosConEstaProduccion = sumatoriaMililitrosEsperados + mililitrosEstimados;

                //verificar que la cantidad de botellas en stock alcance para todas las producciones en proceso
                var botellasNecesariasConEstaProduccion = Math.Ceiling(sumatoriaMililitrosEsperadosConEstaProduccion / medidaEnvase);

                if (botellasNecesariasConEstaProduccion > stockBotellas)
                {
                    return BadRequest(new { message = "No hay suficientes botellas de 355 ml en stock para la producción." });
                }


                var produccion = new Produccion
                {
                    FechaProduccion = DateTime.Now,
                    FechaProximoPaso = DateTime.Now,
                    Mensaje = "",
                    Estatus = 1, //solicitud
                    NumeroTandas = solicitudDto.NumeroTandas,
                    IdReceta = solicitudDto.IdReceta,
                    FechaSolicitud = DateTime.Now,
                    IdUsuarioSolicitud = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "a0a0aaa0-0000-0aa0-0000-00aaa0aa0a00",
                    IdUsuarioProduccion = solicitudDto.IdUsuario,
                    Paso = 0
                };

                _context.Producciones.Add(produccion);
                await _context.SaveChangesAsync();

                bool recetaSePuedeProcesar = true;
                string mensajeInsumosCantidadesFaltantes = "";

                foreach (var ingrediente in receta.IngredientesReceta)
                {
                    var cantidadNecesaria = ingrediente.Cantidad * produccion.NumeroTandas;
                    var lotesInsumo = await _context.LotesInsumos
                        .Where(li => li.IdInsumo == ingrediente.Id && li.FechaCaducidad > DateTime.Today)
                        .OrderBy(li => li.FechaCaducidad)
                        .ToListAsync();

                    foreach (var lote in lotesInsumo)
                    {
                        if (cantidadNecesaria <= 0)
                            break;

                        var cantidadUtilizada = Math.Min(lote.Cantidad, cantidadNecesaria);
                        lote.Cantidad -= cantidadUtilizada;
                        cantidadNecesaria -= cantidadUtilizada;

                        var produccionLoteInsumo = new ProduccionLoteInsumo
                        {
                            IdProduccion = produccion.Id,
                            IdLoteInsumo = lote.Id,
                            Cantidad = cantidadUtilizada
                        };
                        _context.ProduccionLoteInsumos.Add(produccionLoteInsumo);
                        _context.LotesInsumos.Update(lote);
                    }

                    if (cantidadNecesaria > 0)
                    {
                        recetaSePuedeProcesar = false;
                        mensajeInsumosCantidadesFaltantes += $"Faltan {cantidadNecesaria} {ingrediente.UnidadMedida} del insumo {ingrediente.Nombre}\n";
                    }
                }

                if (!recetaSePuedeProcesar)
                {
                    //cancelar los cambios
                    transaction.Rollback();

                    return BadRequest(mensajeInsumosCantidadesFaltantes);
                }
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                try
                {

                    var s = "";
                    if (produccion.NumeroTandas > 1)
                    {
                        s = "s";
                    }
                    Notificacion notificacion = new Notificacion
                    {
                        IdUsuario = produccion.IdUsuarioSolicitud,
                        Fecha = DateTime.Now,
                        Tipo = 5,
                        Mensaje = "Se ha hecho una solicitud de producción de " + produccion.NumeroTandas + $" tanda{s} de " + receta.Nombre,
                        Visto = false
                    };

                    _context.Notificaciones.Add(notificacion);

                    Notificacion notificacion2 = new Notificacion
                    {
                        IdUsuario = produccion.IdUsuarioProduccion,
                        Fecha = DateTime.Now,
                        Tipo = 5,
                        Mensaje = "Se ha hecho una solicitud de producción de " + produccion.NumeroTandas + $" tanda{s} de " + receta.Nombre,
                        Visto = false
                    };

                    _context.Notificaciones.Add(notificacion2);
                    await _context.SaveChangesAsync();
                }
                catch (Exception ex)
                {

                }

                return Ok(new { id = produccion.Id, message = "Solicitud de produccion de receta realizada. Verifique la reduccion de insumos." });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(StatusCodes.Status500InternalServerError, "Error al procesar la solicitud de producción: " + ex.Message);
            }
        }

        // POST: api/Produccion/comenzarProduccionMayorista/5
        [HttpPost("comenzarProduccionMayorista/{idProduccion}")]
        public async Task<IActionResult> comenzarProduccionMayorista(int idProduccion)
        {

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                //traer la produccion
                var produccion = await _context.Producciones
                    .Include(p => p.Receta)
                        .ThenInclude(r => r.IngredientesReceta)
                    .Include(p => p.ProduccionLoteInsumos)
                    .FirstOrDefaultAsync(p => p.Id == idProduccion);



                if (produccion.Receta == null)
                {
                    return NotFound("Receta no encontrada.");
                }


                //calcular el numero de botella de 355 ml que se pueden llenar y si no hay stock de botellas rechazar la solicitud
                var medidaEnvase = 355;
                var litrosEstimados = produccion.Receta.LitrosEstimados * produccion.NumeroTandas;
                var mililitrosEstimados = litrosEstimados * 1000;

                var botellasNecesarias = Math.Ceiling(mililitrosEstimados / medidaEnvase);

                //consultar el stock de botellas con Id = 2 en la tabla de insumos y lotes de insumos
                var stockBotellas = await _context.LotesInsumos
                    .Where(li => li.IdInsumo == 2 && li.FechaCaducidad > DateTime.Today)
                    .SumAsync(li => li.Cantidad);


                if (botellasNecesarias > stockBotellas)
                {
                    return BadRequest(new { message = "No hay suficientes botellas de 355 ml en stock para la producción." });
                }

                //consultar todas todas las producciones en estatus 1, 2 y 3
                var produccionesEnProceso = await _context.Producciones
                    .Where(p => p.Estatus == 1 || p.Estatus == 2 || p.Estatus == 3)
                    .ToListAsync();

                var sumatoriaMililitrosEsperados = 0.0;

                //recorrer las producciones en proceso y sumar los litros esperados de cada una de ellas
                foreach (var prod in produccionesEnProceso)
                {

                    //hacer una consulta para obtener los litros estimados de la receta de cada produccion
                    var recetaProdLitrosEstimados = await _context.Recetas
                        .Where(r => r.Id == prod.IdReceta)
                        .Select(r => new
                        {
                            LitrosEstimados = r.LitrosEstimados
                        })
                        .FirstOrDefaultAsync();

                    sumatoriaMililitrosEsperados += Math.Ceiling((float)recetaProdLitrosEstimados.LitrosEstimados * prod.NumeroTandas * 1000);
                }

                var sumatoriaMililitrosEsperadosConEstaProduccion = sumatoriaMililitrosEsperados + mililitrosEstimados;

                //verificar que la cantidad de botellas en stock alcance para todas las producciones en proceso
                var botellasNecesariasConEstaProduccion = Math.Ceiling(sumatoriaMililitrosEsperadosConEstaProduccion / medidaEnvase);

                if (botellasNecesariasConEstaProduccion > stockBotellas)
                {
                    return BadRequest(new { message = "No hay suficientes botellas de 355 ml en stock para la producción." });
                }


                bool recetaSePuedeProcesar = true;
                string mensajeInsumosCantidadesFaltantes = "";

                //obtener los ingredientes receta con su insumo pertenecientes a produccion.IdReceta
                var listaIngredientesReceta = await _context.IngredientesReceta
                    .Where(ir => ir.IdReceta == produccion.IdReceta)
                    .Select(ir => new
                    {
                        IdInsumo = ir.IdInsumo,
                        Cantidad = ir.Cantidad,
                        Insumo = ir.Insumo
                    })
                    .ToListAsync();

                foreach (var ingrediente in listaIngredientesReceta)
                {
                    if (ingrediente.Insumo == null)
                    {
                        return NotFound("Insumo no encontrado.");
                    }
                    var cantidadNecesaria = ingrediente.Cantidad * produccion.NumeroTandas;
                    var lotesInsumo = await _context.LotesInsumos
                        .Where(li => li.IdInsumo == ingrediente.IdInsumo && li.FechaCaducidad > DateTime.Today)
                        .OrderBy(li => li.FechaCaducidad)
                        .ToListAsync();

                    foreach (var lote in lotesInsumo)
                    {
                        if (cantidadNecesaria <= 0)
                            break;

                        var cantidadUtilizada = Math.Min(lote.Cantidad, cantidadNecesaria);
                        lote.Cantidad -= cantidadUtilizada;
                        cantidadNecesaria -= cantidadUtilizada;

                        var produccionLoteInsumo = new ProduccionLoteInsumo
                        {
                            IdProduccion = produccion.Id,
                            IdLoteInsumo = lote.Id,
                            Cantidad = cantidadUtilizada
                        };
                        _context.ProduccionLoteInsumos.Add(produccionLoteInsumo);
                        _context.LotesInsumos.Update(lote);
                    }

                    if (cantidadNecesaria > 0)
                    {
                        recetaSePuedeProcesar = false;
                        mensajeInsumosCantidadesFaltantes += $"Faltan {cantidadNecesaria} {ingrediente.Insumo.UnidadMedida} del insumo {ingrediente.Insumo.Nombre}\n";
                    }
                }

                if (!recetaSePuedeProcesar)
                {
                    //cancelar los cambios
                    transaction.Rollback();

                    return BadRequest(mensajeInsumosCantidadesFaltantes);
                }

                produccion.Estatus = 2;


                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                try
                {

                    var s = "";
                    if (produccion.NumeroTandas > 1)
                    {
                        s = "s";
                    }
                    Notificacion notificacion = new Notificacion
                    {
                        IdUsuario = produccion.IdUsuarioSolicitud,
                        Fecha = DateTime.Now,
                        Tipo = 5,
                        Mensaje = "Se ha empezado producción de " + produccion.NumeroTandas + $" tanda{s} de " + produccion.Receta.Nombre,
                        Visto = false
                    };

                    _context.Notificaciones.Add(notificacion);

                    Notificacion notificacion2 = new Notificacion
                    {
                        IdUsuario = produccion.IdUsuarioProduccion,
                        Fecha = DateTime.Now,
                        Tipo = 5,
                        Mensaje = "Se ha comenzado producción de " + produccion.NumeroTandas + $" tanda{s} de " + produccion.Receta.Nombre,
                        Visto = false
                    };

                    _context.Notificaciones.Add(notificacion2);
                    await _context.SaveChangesAsync();
                }
                catch (Exception ex)
                {

                }

                return Ok(new { id = produccion.Id, message = "Comienzo de produccion de receta realizada. Verifique la reduccion de insumos." });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(StatusCodes.Status500InternalServerError, "Error al procesar la solicitud de producción: " + ex.Message);
            }
        }


        [HttpPost("resolicitar/{id}")]
        public async Task<IActionResult> RetryProduction(int id)
        {
            var production = await _context.Producciones.FindAsync(id);

            if (production == null)
            {
                return NotFound(new { Message = "Producción no encontrada." });
            }

            if (production.Estatus != 5 && production.Estatus != 6)
            {
                return BadRequest(new { Message = "La producción tiene un estatus diferente a rechazado." });
            }
            if (production.Estatus == 1)
            {
                return BadRequest(new { Message = "La producción ya ha sido solicitada." });
            }
            production.Estatus = 1;
            await _context.SaveChangesAsync();

            try
            {
                //encontrar el nombre de la receta
                var receta = await _context.Recetas.FindAsync(production.IdReceta);
                var s = "";
                if (production.NumeroTandas > 1)
                {
                    s = "s";
                }

                Notificacion notificacion = new Notificacion
                {
                    IdUsuario = production.IdUsuarioSolicitud,
                    Fecha = DateTime.Now,
                    Tipo = 5,
                    Mensaje = "Se ha vuelto a solicitar producción de " + production.NumeroTandas + $" tanda{s} de " + receta?.Nombre,
                    Visto = false
                };

                _context.Notificaciones.Add(notificacion);

                Notificacion notificacion2 = new Notificacion
                {
                    IdUsuario = production.IdUsuarioProduccion,
                    Fecha = DateTime.Now,
                    Tipo = 5,
                    Mensaje = "Se ha vuelto a solicitar producción de " + production.NumeroTandas + $" tanda{s} de " + receta?.Nombre,
                    Visto = false
                };

                _context.Notificaciones.Add(notificacion2);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex) { }

            return Ok(new { Message = "Produccion solicitada nuevamente." });
        }

        // DELETE: api/Produccion/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> CancelarSolicitudProduccion(int id)
        {
            var produccion = await _context.Producciones
                .Include(p => p.Receta)
                .ThenInclude(r => r.IngredientesReceta)
                .ThenInclude(ir => ir.Insumo)
                .Include(p => p.ProduccionLoteInsumos)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (produccion == null)
            {
                return NotFound(new { message = "Solicitud de producción no encontrada." });
            }

            if (produccion.Estatus != 1 && produccion.Estatus != 5 && produccion.Estatus != 6)
            {
                return BadRequest(new { message = "La solicitud no se encuentra en un estado cancelable." });
            }

            foreach (var prodLoteInsumo in produccion.ProduccionLoteInsumos)
            {
                var loteInsumo = await _context.LotesInsumos.FindAsync(prodLoteInsumo.IdLoteInsumo);
                if (loteInsumo != null)
                {
                    loteInsumo.Cantidad += prodLoteInsumo.Cantidad;
                }
            }

            _context.Producciones.Remove(produccion);
            await _context.SaveChangesAsync();

            try
            {
                //encontrar el nombre de la receta
                var receta = await _context.Recetas.FindAsync(produccion.IdReceta);
                var s = "";
                if (produccion.NumeroTandas > 1)
                {
                    s = "s";
                }

                Notificacion notificacion = new Notificacion
                {
                    IdUsuario = produccion.IdUsuarioSolicitud,
                    Fecha = DateTime.Now,
                    Tipo = 5,
                    Mensaje = "Se canceló la producción de " + produccion.NumeroTandas + $" tanda{s} de " + receta?.Nombre,
                    Visto = false
                };

                _context.Notificaciones.Add(notificacion);

                Notificacion notificacion2 = new Notificacion
                {
                    IdUsuario = produccion.IdUsuarioProduccion,
                    Fecha = DateTime.Now,
                    Tipo = 5,
                    Mensaje = "Se canceló la producción de " + produccion.NumeroTandas + $" tanda{s} de " + receta?.Nombre,
                    Visto = false
                };

                _context.Notificaciones.Add(notificacion2);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex) { }

            return Ok(new { message = "Produccion cancelada. Insumos devueltos al almacen." });
        }

        [HttpPost("almacenar/{id}")]
        public async Task<IActionResult> FinalizeProduction(int id, FinalizarProduccionDto finalizeDto)
        {
            var production = await _context.Producciones
                .Include(p => p.Receta)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (production == null)
            {
                return NotFound(new { Message = "Producción no encontrada." });
            }

            await ActualizarCostoReceta(production.IdReceta);

            if (production.Estatus != 3)
            {
                return BadRequest(new { Message = "La producción no se encuentra en espera de almacenamiento." });
            }

            production.Estatus = 4;
            production.Mensaje = finalizeDto.Mensaje;
            production.FechaProduccion = System.DateTime.Now;

            if (finalizeDto.ProduccionFallida == true)
            {
                production.MermaLitros = production.Receta.LitrosEstimados * production.NumeroTandas;
                production.LitrosFinales = 0;
                _context.Entry(production).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                Notificacion notificacion = new Notificacion
                {
                    IdUsuario = production.IdUsuarioSolicitud,
                    Fecha = DateTime.Now,
                    Tipo = 5,
                    Mensaje = $"Se almacenó una producción como fallida",
                    Visto = false
                };

                _context.Notificaciones.Add(notificacion);
                await _context.SaveChangesAsync();

                //si la produccion es mayorista resolicitar la produccion
                if (production.EsMayorista != null && production.EsMayorista == true)
                {
                    var produccion = new Produccion
                    {
                        FechaProduccion = DateTime.Now,
                        FechaProximoPaso = DateTime.Now,
                        Mensaje = "Es de un pedido mayorista resolicitado",
                        Estatus = 10, //pedido mayorista
                        NumeroTandas = production.NumeroTandas,
                        IdReceta = production.IdReceta,
                        FechaSolicitud = production.FechaSolicitud,
                        IdUsuarioSolicitud = production.IdUsuarioSolicitud,
                        IdUsuarioProduccion = production.IdUsuarioProduccion,
                        Paso = 0,
                        EsMayorista = true,
                        PrecioMayoristaFijado = production.PrecioMayoristaFijado,
                        IdPedidoMayoreo = production.IdPedidoMayoreo,
                        CantidadMayoristaRequerida = production.CantidadMayoristaRequerida,
                        StocksRequeridos = production.StocksRequeridos
                    };
                    _context.Producciones.Add(produccion);
                    await _context.SaveChangesAsync();
                }

                return Ok(new { Message = "Producción perdida." });
            }

            //Valida valores de litros finales y merma
            if (finalizeDto.CalcularMerma == true)
            {
                if (finalizeDto.LitrosFinales == null)
                {
                    return BadRequest(new { Message = "Debe ingresar los litros finales para calcular la merma." });
                }
                if (finalizeDto.MermaLitros != null)
                {
                    return BadRequest(new { Message = "No se puede calcular la merma si ya se ingreso un valor." });
                }

                var litrosEstimados = production.Receta.LitrosEstimados * production.NumeroTandas;
                if (finalizeDto.LitrosFinales > litrosEstimados * 1.02)
                {
                    return BadRequest(new { Message = "Los litros finales estan desfazados, más 2% de la estimación de la receta. Confirma las mediciones." });
                }
                else if (finalizeDto.LitrosFinales < litrosEstimados * 0.98)
                {
                    return BadRequest(new { Message = "Los litros finales estan desfazados, menos 2% de la estimación de la receta. Confirma las mediciones." });
                }
                production.LitrosFinales = finalizeDto.LitrosFinales;
                production.MermaLitros = finalizeDto.LitrosFinales - production.Receta.LitrosEstimados * production.NumeroTandas;
            }
            else
            {
                if (finalizeDto.MermaLitros == null)
                {
                    return BadRequest(new { Message = "Debe ingresar la merma o escoger que se calcule." });
                }

                if (finalizeDto.LitrosFinales != null)
                {
                    //si la suma de litrosFinales y mermaLitros se desvia mas del 2% de los litros estimados de la receta
                    var afirmacionLitros = finalizeDto.LitrosFinales + finalizeDto.MermaLitros;
                    var litrosEstimados = production.Receta.LitrosEstimados * production.NumeroTandas;
                    if (afirmacionLitros > litrosEstimados * 1.02)
                    {
                        return BadRequest(new { Message = "La suma de litros finales y merma esta desfazada, más 2% de la estimación de la receta. Confirma las mediciones." });
                    }
                    else if (afirmacionLitros < litrosEstimados * 0.98)
                    {
                        return BadRequest(new { Message = "La suma de litros finales y merma esta desfazada, menos 2% de la estimación de la receta. Confirma las mediciones." });
                    }
                    production.MermaLitros = finalizeDto.MermaLitros;
                    production.LitrosFinales = finalizeDto.LitrosFinales;
                }
                else if (finalizeDto.LitrosFinales == null)
                {
                    //verificar que la merma no sea mayor a los litros estimados
                    if (finalizeDto.MermaLitros > production.Receta.LitrosEstimados * production.NumeroTandas)
                    {
                        return BadRequest(new { Message = "La merma no puede ser mayor a los litros estimados de la receta." });
                    }
                    production.MermaLitros = finalizeDto.MermaLitros;
                    production.LitrosFinales = (production.Receta.LitrosEstimados * production.NumeroTandas) - finalizeDto.MermaLitros;
                }
            }

            if (production.MermaLitros < 0)
            {
                production.MermaLitros = 0;
            }
            if (production.LitrosFinales <= 0)
            {
                production.MermaLitros = production.Receta.LitrosEstimados * production.NumeroTandas;
                production.LitrosFinales = 0;
                _context.Entry(production).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                Notificacion notificacion = new Notificacion
                {
                    IdUsuario = production.IdUsuarioSolicitud,
                    Fecha = DateTime.Now,
                    Tipo = 5,
                    Mensaje = $"Se almacenó una producción como fallida",
                    Visto = false
                };

                _context.Notificaciones.Add(notificacion);
                await _context.SaveChangesAsync();

                //si la produccion es mayorista resolicitar la produccion
                if (production.EsMayorista != null && production.EsMayorista == true)
                {

                    var produccion = new Produccion
                    {
                        FechaProduccion = DateTime.Now,
                        FechaProximoPaso = DateTime.Now,
                        Mensaje = "Es de un pedido mayorista resolicitado",
                        Estatus = 10, //pedido mayorista
                        NumeroTandas = production.NumeroTandas,
                        IdReceta = production.IdReceta,
                        FechaSolicitud = production.FechaSolicitud,
                        IdUsuarioSolicitud = production.IdUsuarioSolicitud,
                        IdUsuarioProduccion = production.IdUsuarioProduccion,
                        Paso = 0,
                        EsMayorista = true,
                        PrecioMayoristaFijado = production.PrecioMayoristaFijado,
                        IdPedidoMayoreo = production.IdPedidoMayoreo,
                        CantidadMayoristaRequerida = production.CantidadMayoristaRequerida,
                        StocksRequeridos = production.StocksRequeridos
                    };
                    _context.Producciones.Add(produccion);
                    await _context.SaveChangesAsync();
                }

                return Ok(new { Message = "Producción finalizada como fallida." });
            }

            var noApartar = false;
            if (production.EsMayorista != null && production.EsMayorista == true)
            {

                //verificar que la produccion alcance para la cantidad mayorista requerida
                if (production.CantidadMayoristaRequerida > Math.Floor((double)(production.LitrosFinales / 0.355)))
                {
                    noApartar = true;
                    var produccion = new Produccion
                    {
                        FechaProduccion = DateTime.Now,
                        FechaProximoPaso = DateTime.Now,
                        Mensaje = "Es de un pedido mayorista resolicitado",
                        Estatus = 10, //pedido mayorista
                        NumeroTandas = production.NumeroTandas,
                        IdReceta = production.IdReceta,
                        FechaSolicitud = production.FechaSolicitud,
                        IdUsuarioSolicitud = production.IdUsuarioSolicitud,
                        IdUsuarioProduccion = production.IdUsuarioProduccion,
                        IdUsuarioMayorista = production.IdUsuarioMayorista,
                        Paso = 0,
                        EsMayorista = true,
                        PrecioMayoristaFijado = production.PrecioMayoristaFijado,
                        IdPedidoMayoreo = production.IdPedidoMayoreo,
                        CantidadMayoristaRequerida = production.CantidadMayoristaRequerida,
                        StocksRequeridos = production.StocksRequeridos
                    };
                    _context.Producciones.Add(produccion);
                    await _context.SaveChangesAsync();
                }
            }

            //consultar el numero de botellas id = 2 en stock
            var numBotellas = await _context.LotesInsumos
                 .Where(li => li.IdInsumo == 2 && li.FechaCaducidad > DateTime.Today)
                 .SumAsync(li => li.Cantidad);

            production.CostoProduccion = production.NumeroTandas * production.Receta.CostoProduccion;

            var medidaEnvase = finalizeDto.MedidaEnvase ?? 355;
            medidaEnvase = medidaEnvase == 0 ? 355 : medidaEnvase;

            float mililitrosParaEnvasar = (production.LitrosFinales ?? 0) * 1000;

            float proporcionLitrosStock = mililitrosParaEnvasar / medidaEnvase;

            int cantidadStock = (int)Math.Floor(proporcionLitrosStock);
            int BotellasAUsar = cantidadStock;

            if (cantidadStock > numBotellas)
            {
                return BadRequest(new { Message = "No hay suficientes botellas de 355 ml en stock para la producción." });
            }

            //restar las botellas del stock desde la fecha de caducidad mas proxima
            var lotesBotellas = await _context.LotesInsumos
                .Where(li => li.IdInsumo == 2 && li.FechaCaducidad > DateTime.Today)
                .OrderBy(li => li.FechaCaducidad)
                .ToListAsync();

            foreach (var lote in lotesBotellas)
            {
                if (BotellasAUsar <= 0)
                    break;

                var cantidadUtilizada = (int)Math.Min(lote.Cantidad, BotellasAUsar);
                lote.Cantidad -= cantidadUtilizada;
                BotellasAUsar -= cantidadUtilizada;
                _context.LotesInsumos.Update(lote);
            }


            DateTime fechaCaducidad = DateTime.Now.AddDays(production.Receta.TiempoVida);

            float sobranteMl = proporcionLitrosStock - cantidadStock;

            float sobrante = sobranteMl * medidaEnvase / 1000;

            production.MermaLitros = production.MermaLitros + sobrante;

            _context.Entry(production).State = EntityState.Modified;

            var stock = new Stock
            {
                FechaEntrada = System.DateTime.Now,
                FechaCaducidad = fechaCaducidad,
                Cantidad = cantidadStock,
                TipoEnvase = finalizeDto.TipoEnvase,
                MedidaEnvase = medidaEnvase,
                Merma = 0,
                IdProduccion = production.Id,
                IdReceta = production.Receta.Id,
                IdUsuario = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "a0a0aaa0-0000-0aa0-0000-00aaa0aa0a00"
            };

            if (production.EsMayorista != null && production.EsMayorista == true && noApartar == false)
            {
                stock.CantidadApartada = production.CantidadMayoristaRequerida;
                stock.Cantidad = (int)(cantidadStock - production.CantidadMayoristaRequerida);
                stock.IdPedidoMayoreo = production.IdPedidoMayoreo;
                stock.EsMayorista = true;
                stock.PrecioMayoristaFijado = production.PrecioMayoristaFijado;
            }

            _context.Stocks.Add(stock);
            await _context.SaveChangesAsync();

            if (production.EsMayorista != null && production.EsMayorista == true && noApartar == false)
            {
                //contar el numero de producciones con el mismo idPedidoMayoreo


                //traer los stocks con el mismo idPedidoMayoreo
                var stocksMayoreo = await _context.Stocks
                    .Where(s => s.IdPedidoMayoreo == production.IdPedidoMayoreo)
                    .ToListAsync();

                //si el numero de producciones es igual al numero de stocks entonces procesar la venta de mayoreo
                if (production.StocksRequeridos == stocksMayoreo.Count)
                {
                    //Traer el cliente mayorista
                    var pedidoMayoreoParaId = await _context.PedidosMayoreo
                        .Where(p => p.Id == production.IdPedidoMayoreo).FirstOrDefaultAsync();

                    var clienteMayorista = await _context.ClientesMayoristas
                        .Where(p => p.Id == pedidoMayoreoParaId.IdMayorista).FirstOrDefaultAsync();

                    var costoVenta = 0.0;
                    foreach (var stockMayoreo in stocksMayoreo)
                    {
                        costoVenta += (double)stockMayoreo.PrecioMayoristaFijado;
                    }

                    int totalDeCervezas = (int)Math.Floor((decimal)(production.LitrosFinales * 1000) / medidaEnvase);

                    var venta = new Venta
                    {
                        IdUsuario = production.IdUsuarioMayorista,
                        FechaVenta = DateTime.Now,
                        EstatusVenta = EstatusVenta.Recibido,
                        MetodoEnvio = MetodoEnvio.EnvioDomicilio,
                        MetodoPago = MetodoPago.Plazos,

                        NombrePersonaRecibe = clienteMayorista.NombreEmpresa,
                        Calle = clienteMayorista.DireccionEmpresa,
                        Total = (float)costoVenta,
                        TotalCervezas = production.CantidadMayoristaRequerida ?? totalDeCervezas,
                        Mayoreo = true

                    };
                    _context.Ventas.Add(venta);
                    await _context.SaveChangesAsync();

                    foreach (var stockMayoreo in stocksMayoreo)
                    {
                        //generar un detalle de venta
                        var detalleVenta = new DetalleVenta
                        {
                            IdVenta = venta.Id,
                            IdStock = stockMayoreo.Id,
                            Cantidad = stockMayoreo.CantidadApartada,
                            MontoVenta = stockMayoreo.PrecioMayoristaFijado ?? 0,
                            Pack = 0,
                            IdReceta = stockMayoreo.IdReceta
                        };
                        _context.DetallesVenta.Add(detalleVenta);

                    }
                    await _context.SaveChangesAsync();

                    //cambiar el estatus del pedido mayorista a 1
                    var pedidoMayorista = await _context.PedidosMayoreo.FindAsync(production.IdPedidoMayoreo);
                    pedidoMayorista.IdVenta = venta.Id;
                    pedidoMayorista.Estatus = 1;
                    _context.Entry(pedidoMayorista).State = EntityState.Modified;
                    await _context.SaveChangesAsync();

                }
            }


            try
            {
                //encontrar el nombre de la receta
                var receta = await _context.Recetas.FindAsync(production.IdReceta);

                var s = "";
                if (production.NumeroTandas > 1)
                {
                    s = "s";
                }
                Notificacion notificacion = new Notificacion
                {
                    IdUsuario = production.IdUsuarioSolicitud,
                    Fecha = DateTime.Now,
                    Tipo = 5,
                    Mensaje = $"Se almacenó en stock {stock.Cantidad} {stock.TipoEnvase}s de {receta?.Nombre} de {stock.MedidaEnvase} ml",
                    Visto = false
                };

                _context.Notificaciones.Add(notificacion);

                Notificacion notificacion2 = new Notificacion
                {
                    IdUsuario = production.IdUsuarioProduccion,
                    Fecha = DateTime.Now,
                    Tipo = 5,
                    Mensaje = "Se almacenó la producción de " + production.NumeroTandas + $" tanda{s} de " + receta?.Nombre,
                    Visto = false
                };

                _context.Notificaciones.Add(notificacion2);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex) { }

            return Ok(new { Message = "Production finalizada y almacenaje del producto en stock." });
        }

        //metodo para cambiar el usuario asignado a una produccion
        [HttpPut("asignar-usuario/{id}")]
        public async Task<IActionResult> CambiarUsuarioProduccion(int id, [FromBody] UserSelectDto userSelectDto)
        {

            string idUsuario = userSelectDto.IdUsuario;
            var produccion = await _context.Producciones.FindAsync(id);

            if (produccion == null)
            {
                return NotFound(new { Message = "Producción no encontrada." });
            }

            //validar que el usuario exista
            var usuario = await _context.Users.FindAsync(idUsuario);
            if (usuario == null)
            {
                return NotFound(new { Message = "Usuario no encontrado." });
            }

            produccion.IdUsuarioProduccion = idUsuario;
            _context.Entry(produccion).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            try
            {
                //encontrar el nombre de la receta
                var receta = await _context.Recetas.FindAsync(produccion.IdReceta);
                var s = "";
                if (produccion.NumeroTandas > 1)
                {
                    s = "s";
                }

                Notificacion notificacion = new Notificacion
                {
                    IdUsuario = produccion.IdUsuarioSolicitud,
                    Fecha = DateTime.Now,
                    Tipo = 5,
                    Mensaje = "Se re-asignó la producción de " + produccion.NumeroTandas + $" tanda{s} de " + receta?.Nombre,
                    Visto = false
                };

                _context.Notificaciones.Add(notificacion);

                Notificacion notificacion2 = new Notificacion
                {
                    IdUsuario = produccion.IdUsuarioProduccion,
                    Fecha = DateTime.Now,
                    Tipo = 5,
                    Mensaje = "Se le re-asignó la producción de " + produccion.NumeroTandas + $" tanda{s} de " + receta?.Nombre,
                    Visto = false
                };

                _context.Notificaciones.Add(notificacion2);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex) { }

            return Ok(new { Message = "Usuario de producción cambiado." });
        }


        private async Task ActualizarCostoReceta(int idReceta)
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

                if (nuevoCostoLitro > receta.PrecioLitro)
                {
                    receta.PrecioLitro = nuevoCostoLitro;
                }
                receta.CostoProduccion = costoTotal;

                _context.Entry(receta).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }
        }

        //Metodos del cocinero
        [HttpGet("aceptar-solicitud/{idSolicitud}")]
        public IActionResult AceptarSolicitud(int idSolicitud)
        {
            var produccion = _context.Producciones
                .FirstOrDefault(p => p.Id == idSolicitud);

            if (produccion != null)
            {
                if (produccion.Estatus != 1)
                {
                    return Ok(new { message = "La solicitud se encuentra en un estatus diferente a solicitud realizada, no se puede aceptar" });
                }

                produccion.Estatus = 2;

                var ingredientesReceta = _context.IngredientesReceta
                    .Where(ir => ir.IdReceta == produccion.IdReceta)
                    .ToList();

                var receta = _context.Recetas
                    .FirstOrDefault(r => r.Id == produccion.IdReceta);

                var mensajeReceta = $"Descripción receta: \n{receta.Descripcion}\n\n";
                mensajeReceta += "Los ingredientes para esta receta son:\n";
                foreach (var ingredienteReceta in ingredientesReceta)
                {
                    var insumo = _context.Insumos
                        .FirstOrDefault(i => i.Id == ingredienteReceta.IdInsumo);

                    var cantidadNecesaria = ingredienteReceta.Cantidad * produccion.NumeroTandas;

                    string unidadMedida = insumo.UnidadMedida;
                    double cantidadFormateada = cantidadNecesaria;

                    mensajeReceta += $"{insumo.Nombre}: {cantidadFormateada:F2} {unidadMedida}\n";
                }

                _context.SaveChanges();

                try
                {

                    var s = "";
                    if (produccion.NumeroTandas > 1)
                    {
                        s = "s";
                    }

                    Notificacion notificacion = new Notificacion
                    {
                        IdUsuario = produccion.IdUsuarioSolicitud,
                        Fecha = DateTime.Now,
                        Tipo = 5,
                        Mensaje = "El usuario operador aceptó la producción de " + produccion.NumeroTandas + $" tanda{s} de " + receta?.Nombre,
                        Visto = false
                    };

                    _context.Notificaciones.Add(notificacion);

                    Notificacion notificacion2 = new Notificacion
                    {
                        IdUsuario = produccion.IdUsuarioProduccion,
                        Fecha = DateTime.Now,
                        Tipo = 5,
                        Mensaje = "Aceptó la producción de " + produccion.NumeroTandas + $" tanda{s} de " + receta?.Nombre,
                        Visto = false
                    };

                    _context.Notificaciones.Add(notificacion2);
                    _context.SaveChangesAsync();
                }
                catch (Exception ex) { }

                return Ok(new { message = mensajeReceta });
            }
            else
            {
                return Ok(new { message = "No se encontró la solicitud de producción con los datos proporcionados" });
            }
        }

        [HttpPost("rechazar-solicitud/{idSolicitud}")]
        public IActionResult RechazarSolicitud(int idSolicitud, [FromForm] string mensaje)
        {
            var produccion = _context.Producciones
                .FirstOrDefault(p => p.Id == idSolicitud);

            if (produccion != null && produccion.Estatus == 1)
            {
                produccion.Mensaje = mensaje;
                produccion.Estatus = 5;

                _context.SaveChanges();

                try
                {
                    var receta = _context.Recetas
                        .FirstOrDefault(r => r.Id == produccion.IdReceta)?.Nombre;

                    var s = "";
                    if (produccion.NumeroTandas > 1)
                    {
                        s = "s";
                    }

                    Notificacion notificacion = new Notificacion
                    {
                        IdUsuario = produccion.IdUsuarioSolicitud,
                        Fecha = DateTime.Now,
                        Tipo = 5,
                        Mensaje = "El usuario operador rechazó la producción de " + produccion.NumeroTandas + $" tanda{s} de " + receta,
                        Visto = false
                    };

                    _context.Notificaciones.Add(notificacion);

                    Notificacion notificacion2 = new Notificacion
                    {
                        IdUsuario = produccion.IdUsuarioProduccion,
                        Fecha = DateTime.Now,
                        Tipo = 5,
                        Mensaje = "Rechazó la producción de " + produccion.NumeroTandas + $" tanda{s} de " + receta,
                        Visto = false
                    };

                    _context.Notificaciones.Add(notificacion2);
                    _context.SaveChangesAsync();
                }
                catch (Exception ex) { }

                return Ok(new { message = "Solicitud rechazada correctamente" });
            }
            else
            {
                return Ok(new { message = "La solicitud se encuentra en un estatus diferente a solicitud realizada, no se puede rechazar" });

            }
        }

        [HttpPost("posponer-solicitud/{idSolicitud}")]
        public IActionResult PosponerSolicitud(int idSolicitud, [FromForm] string mensaje)
        {
            var produccion = _context.Producciones
                .FirstOrDefault(p => p.Id == idSolicitud);

            if (produccion != null && produccion.Estatus == 1)
            {
                produccion.Mensaje = mensaje;
                produccion.Estatus = 6;

                _context.SaveChanges();

                try
                {
                    var receta = _context.Recetas
                        .FirstOrDefault(r => r.Id == produccion.IdReceta)?.Nombre;

                    var s = "";
                    if (produccion.NumeroTandas > 1)
                    {
                        s = "s";
                    }
                    Notificacion notificacion = new Notificacion
                    {
                        IdUsuario = produccion.IdUsuarioSolicitud,
                        Fecha = DateTime.Now,
                        Tipo = 5,
                        Mensaje = "El usuario operador pospuso la producción de " + produccion.NumeroTandas + $" tanda{s} de " + receta,
                        Visto = false
                    };

                    _context.Notificaciones.Add(notificacion);

                    Notificacion notificacion2 = new Notificacion
                    {
                        IdUsuario = produccion.IdUsuarioProduccion,
                        Fecha = DateTime.Now,
                        Tipo = 5,
                        Mensaje = "Pospuso la producción de " + produccion.NumeroTandas + $" tanda{s} de " + receta,
                        Visto = false
                    };

                    _context.Notificaciones.Add(notificacion2);
                    _context.SaveChangesAsync();
                }
                catch (Exception ex) { }

                return Ok(new { message = "Solicitud pospuesta correctamente" });
            }
            else
            {
                return Ok(new { message = "La solicitud se encuentra en un estatus diferente a solicitud realizada, no se puede posponer" });
            }
        }


        [HttpPost("AvanzarPaso")]
        public async Task<IActionResult> AvanzarPasoProduccion([FromBody] AvanzarPasoProduccionDto dto)
        {
            var produccion = await _context.Producciones
                .Include(p => p.Receta)
                    .ThenInclude(r => r.PasosReceta)
                .FirstOrDefaultAsync(p => p.Id == dto.IdProduccion);

            if (produccion == null)
            {
                return NotFound(new { message = "Producción no encontrada." });
            }

            if (produccion.Estatus != 2)
            {
                return BadRequest(new { message = "El estatus de la producción no es válido para avanzar en los pasos." });
            }

            var pasos = produccion.Receta.PasosReceta.OrderBy(p => p.Orden).ToList();
            var pasoActual = produccion.Paso;
            var pasoSiguiente = pasos.FirstOrDefault(p => p.Orden > pasoActual);

            if (produccion.FechaProximoPaso > DateTime.Now)
            {
                return BadRequest(new { message = "Todavía no se ha alcanzado la fecha del próximo paso. No es posible avanzar." });
            }

            if (pasoActual == 0)
            {
                pasoSiguiente = pasos.FirstOrDefault();
            }

            if (pasoSiguiente == null)
            {
                produccion.Estatus = 3; // Producción en espera de almacenamiento
                produccion.Mensaje = dto.Mensaje ?? "Producción completada. En espera de almacenamiento.";
            }
            else
            {
                produccion.Paso = pasoSiguiente.Orden;
                produccion.Mensaje = pasoSiguiente.Descripcion;
                produccion.FechaProximoPaso = DateTime.Now.AddHours(pasoSiguiente.Tiempo);

            }

            if (dto.MermaLitros != null)
            {
                produccion.MermaLitros += dto.MermaLitros;
            }
            produccion.FechaProduccion = DateTime.Now;

            await _context.SaveChangesAsync();

            var message = "Producción actualizada al siguiente paso.";
            if (produccion.Estatus == 3)
            {
                message = "Producción completada. En espera de almacenamiento.";
            }

            try
            {
                var receta = _context.Recetas
                    .FirstOrDefault(r => r.Id == produccion.IdReceta)?.Nombre;

                var s = "";
                if (produccion.NumeroTandas > 1)
                {
                    s = "s";
                }

                if (produccion.Estatus == 3)
                {
                    Notificacion notificacion = new Notificacion
                    {
                        IdUsuario = produccion.IdUsuarioProduccion,
                        //poner la fecha de la notificacion con la fecha actual mas el tiempo del paso
                        Fecha = DateTime.Now,
                        Tipo = 5,
                        Mensaje = $"Ya finalizaste la producción de {produccion.NumeroTandas} tanda{s} de {receta}",
                        Visto = false
                    };
                    _context.Notificaciones.Add(notificacion);

                    Notificacion notificacion2 = new Notificacion
                    {
                        IdUsuario = produccion.IdUsuarioSolicitud,
                        //poner la fecha de la notificacion con la fecha actual mas el tiempo del paso
                        Fecha = DateTime.Now,
                        Tipo = 5,
                        Mensaje = $"La producción de {produccion.NumeroTandas} tanda{s} de {receta} espera almacenaje",
                        Visto = false
                    };
                    _context.Notificaciones.Add(notificacion2);
                }
                else
                {
                    Notificacion notificacion = new Notificacion
                    {
                        IdUsuario = produccion.IdUsuarioProduccion,
                        //poner la fecha de la notificacion con la fecha actual mas el tiempo del paso
                        Fecha = DateTime.Now.AddHours(pasoSiguiente.Tiempo),
                        Tipo = 5,
                        Mensaje = $"Ya se puede avanzar en la producción de {produccion.NumeroTandas} tanda{s} de {receta}",
                        Visto = false
                    };
                    _context.Notificaciones.Add(notificacion);
                }
                await _context.SaveChangesAsync();
            }
            catch (Exception ex) { }

            return Ok(new { message });
        }


        [HttpGet("Solicitud")]
        public async Task<ActionResult<IEnumerable<ConsultaSolicitudesOperador>>> GetProduccionesSolicitud()
        {

            //obtener la id del usuario actual
            var idUsuario = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var producciones = await _context.Producciones
                .Where(p => p.Estatus == 1 && p.IdUsuarioProduccion == idUsuario)
                .Include(p => p.Receta)
                    .ThenInclude(r => r.IngredientesReceta)
                    .ThenInclude(ir => ir.Insumo)
                .Select(p => new ConsultaSolicitudesOperador
                {
                    Id = p.Id,
                    Estatus = p.Estatus,
                    NumeroTandas = p.NumeroTandas,
                    IdReceta = p.IdReceta,
                    NombreReceta = p.Receta.Nombre,
                    FechaSolicitud = p.FechaSolicitud,
                    IdUsuarioSolicitud = p.IdUsuarioSolicitud
                })
                .ToListAsync();

            return Ok(producciones);
        }

        [HttpGet("Aceptadas")]
        public async Task<ActionResult<IEnumerable<ConsultaProduccionesOperador>>> GetProduccionesAceptadas()
        {
            var idUsuario = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var producciones = await _context.Producciones
                .Where(p => p.Estatus == 2 && p.IdUsuarioProduccion == idUsuario)
                .Include(p => p.Receta)
                    .ThenInclude(r => r.IngredientesReceta)
                    .ThenInclude(ir => ir.Insumo)
                .Select(p => new ConsultaProduccionesOperador
                {
                    Id = p.Id,
                    Estatus = p.Estatus,
                    NumeroTandas = p.NumeroTandas,
                    IdReceta = p.IdReceta,
                    FechaProximoPaso = p.FechaProximoPaso,
                    NombreReceta = p.Receta.Nombre,
                    FechaSolicitud = p.FechaSolicitud,
                    Paso = p.Paso,
                    DescripcionPaso = _context.PasosRecetas
                        .Where(pp => pp.IdReceta == p.IdReceta && pp.Orden == p.Paso)
                        .Select(pp => pp.Descripcion)
                        .FirstOrDefault() ?? "Sin descripción",
                    IdUsuarioSolicitud = p.IdUsuarioSolicitud,
                    IdUsuarioProduccion = p.IdUsuarioProduccion,
                    EsMayorista = p.EsMayorista ?? false
                })
                .ToListAsync();

            return Ok(producciones);
        }

    }
}
