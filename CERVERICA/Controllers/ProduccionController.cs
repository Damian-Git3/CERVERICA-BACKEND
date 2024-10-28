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
    public class ProduccionController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ProduccionController> _logger;

        public ProduccionController(ApplicationDbContext context, ILogger<ProduccionController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: api/Produccion
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProduccionesDto>>> GetProducciones()
        {
            var producciones = await _context.Producciones
                .Where(p => p.Estatus != 4)
                .Include(p => p.Receta)
                    .ThenInclude(r => r.IngredientesReceta)
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
                    .ThenInclude(r => r.IngredientesReceta)
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
                return Ok(new { Message = "Producción finalizada como fallida." });
            }

            production.CostoProduccion = production.NumeroTandas * production.Receta.CostoProduccion;

            var medidaEnvase = finalizeDto.MedidaEnvase ?? 355;
            medidaEnvase = medidaEnvase == 0 ? 355 : medidaEnvase;

            float mililitrosParaEnvasar = (production.LitrosFinales ?? 0) * 1000;

            float proporcionLitrosStock = mililitrosParaEnvasar / medidaEnvase;

            int cantidadStock = (int)proporcionLitrosStock;

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

            _context.Stocks.Add(stock);
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

        //[HttpPost("cambiarestdo")]
        //public async Task<IActionResult> CambiarEstadoProduccion(int idproduccion, byte estatus)
        //{
        //    var produccion = await _context.Producciones.FindAsync(idproduccion);

        //    produccion.Estatus = estatus;

        //    _context.Entry(produccion).State = EntityState.Modified;
        //    await _context.SaveChangesAsync();

        //    return Ok(new { Message = "Estatus de producción a 3 otra vez." });
        //}

        ////vaciar tabla stock
        //[HttpPost("vaciarstock")]
        //public async Task<IActionResult> VaciarStock()
        //{
        //    var stocks = await _context.Stocks.ToListAsync();

        //    _context.Stocks.RemoveRange(stocks);
        //    await _context.SaveChangesAsync();

        //    return Ok(new { Message = "Stock vaciado." });
        //}

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
                .Where(p => p.Estatus == 2 && p.IdUsuarioProduccion ==idUsuario)
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
                    IdUsuarioProduccion = p.IdUsuarioProduccion

                })
                .ToListAsync();

            return Ok(producciones);
        }

    }
}
