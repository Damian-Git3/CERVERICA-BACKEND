using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using CERVERICA.Data;
using CERVERICA.Models;
using CERVERICA.Dtos;
using System.Security.Claims;
using System.Diagnostics;

namespace CERVERICA.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize(Roles = "Vendedor")]
    public class ProduccionController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ProduccionController(ApplicationDbContext context)
        {
            _context = context;
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
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
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
                                IdInsumo = ir.IdInsumo,
                                Cantidad = ir.Cantidad,
                                NombreInsumo = ir.Insumo.Nombre,
                                UnidadMedida = ir.Insumo.UnidadMedida
                            }).ToList()
                        })
                        .FirstOrDefaultAsync();

                    if (receta == null)
                    {
                        return NotFound("Receta no encontrada.");
                    }

                    var produccion = new Produccion();

                    produccion.FechaProduccion = DateTime.Now;
                    produccion.FechaProximoPaso = DateTime.Now;
                    produccion.Mensaje = "";
                    produccion.Estatus = 1; //solicitud
                    produccion.NumeroTandas = solicitudDto.NumeroTandas;
                    produccion.IdReceta = solicitudDto.IdReceta;
                    produccion.FechaSolicitud = DateTime.Now;
                    produccion.IdUsuarioSolicitud = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "a0a0aaa0-0000-0aa0-0000-00aaa0aa0a00";
                    produccion.IdUsuarioProduccion = solicitudDto.IdUsuario;
                    produccion.Paso = 0;

                    _context.Producciones.Add(produccion);
                    await _context.SaveChangesAsync();

                    bool recetaSePuedeProcesar = true;
                    string mensajeInsumosCantidadesFaltantes = "";

                    foreach (var ingrediente in receta.IngredientesReceta)
                    {
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
                            mensajeInsumosCantidadesFaltantes += $"Faltan {cantidadNecesaria} {ingrediente.UnidadMedida} del insumo {ingrediente.NombreInsumo}\n";
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

                    return Ok(new { id = produccion.Id, message = "Solicitud de produccion de receta realizada. Verifique la reduccion de insumos." });
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    return StatusCode(StatusCodes.Status500InternalServerError, "Error al procesar la solicitud de producción: " + ex.Message);
                }
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

            if (production.Estatus != 4)
            {
                return BadRequest(new { Message = "La producción tiene un estatus diferente a rechazado." });
            }

            production.Estatus = 1;
            await _context.SaveChangesAsync();

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
                return NotFound("Solicitud de producción no encontrada.");
            }

            if (produccion.Estatus != 1 && produccion.Estatus != 5)
            {
                return BadRequest("La solicitud no se encuentra en un estado cancelable.");
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

            return NoContent();
        }

        [HttpPost("finalizar/{id}")]
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
                return BadRequest(new { Message = "La producción no se encuentra en fabricación." });
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
                return Ok(new { Message = "Producción finalizada como fallida." });
            }

            //Valida valores de litros finales y merma
            if (finalizeDto.CalcularMerma == true)
            {
                if (finalizeDto.LitrosFinales == null)
                {
                    return BadRequest(new { Message = "Debe ingresar los litros finales para calcular la merma." });
                }
                if(finalizeDto.MermaLitros != null)
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
                if(finalizeDto.MermaLitros == null)
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
                return Ok(new { Message = "Producción finalizada como fallida." });
            }
            
            production.CostoProduccion = production.NumeroTandas * production.Receta.CostoProduccion;

            var medidaEnvase = finalizeDto.MedidaEnvase ?? 355;
            medidaEnvase = medidaEnvase == 0 ? 355 : medidaEnvase;

            float mililitrosParaEnvasar = (production.LitrosFinales??0) * 1000;

            float proporcionLitrosStock = mililitrosParaEnvasar / medidaEnvase;

            int cantidadStock = (int)proporcionLitrosStock;

            float sobranteMl = proporcionLitrosStock - cantidadStock;

            float sobrante = sobranteMl * medidaEnvase / 1000;

            production.MermaLitros = production.MermaLitros + sobrante;

            _context.Entry(production).State = EntityState.Modified;

            var stock = new Stock
            {
                FechaEntrada = System.DateTime.Now,
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

            return Ok(new { Message = "Production finalizada y almacenaje del producto en stock." });
        }

        //metodo para cambiar el usuario asignado a una produccion
        [HttpPut("cambiarUsuario/{id}")]
        public async Task<IActionResult> CambiarUsuarioProduccion(int id, [FromBody] string idUsuario)
        {
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

            return Ok(new { Message = "Usuario de producción cambiado." });
        }

        [HttpPost("cambiarestdo")]
        public async Task<IActionResult> CambiarEstadoProduccion()
        {
            var produccion = await _context.Producciones.FindAsync(7);

            produccion.Estatus = 3;

            _context.Entry(produccion).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Estatus de producción a 3 otra vez." });
        }

        //vaciar tabla stock
        [HttpPost("vaciarstock")]
        public async Task<IActionResult> VaciarStock()
        {
            var stocks = await _context.Stocks.ToListAsync();

            _context.Stocks.RemoveRange(stocks);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Stock vaciado." });
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
                if(produccion.Estatus != 1)
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

                return Ok(new { message = mensajeReceta });
            }
            else
            {
                return Ok(new { message = "No se encontró la solicitud de producción con los datos proporcionados" });
            }
        }

        [HttpPost("finalizar-solicitud/{idSolicitud}")]
        public IActionResult FinalizarSolicitud(int idSolicitud, [FromForm] float mermaLitros)
        {
            var produccion = _context.Producciones
                .FirstOrDefault(p => p.Id == idSolicitud);

            if (produccion != null && produccion.Estatus == 2)
            {
                produccion.MermaLitros = mermaLitros;
                produccion.Estatus = 3;

                _context.SaveChanges();

                return Ok(new { message = "Solicitud finalizada correctamente" });
            }
            else
            {
                return Ok(new { message = "La solicitud se encuentra en un estatus diferente a solicitud en preparación, no se puede finalizar" });
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

                return Ok(new { message =  "Solicitud rechazada correctamente"});
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

                return Ok(new { message = "Solicitud pospuesta correctamente" });
            }
            else
            {
                return Ok(new { message = "La solicitud se encuentra en un estatus diferente a solicitud realizada, no se puede posponer" });
            }
        }
    }
}
