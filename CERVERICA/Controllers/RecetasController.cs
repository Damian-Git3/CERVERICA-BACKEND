using CERVERICA.Data;
using CERVERICA.Dtos;
using CERVERICA.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace CERVERICA.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class RecetaController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        private const string AdminRoleName = "Admin";

        private const string RecetaNoExisteMessage = "Receta no existe.";

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
                    Nombre = r.Nombre,
                    Descripcion = r.Descripcion ?? string.Empty,
                    PrecioLitro = r.PrecioLitro,
                    LitrosEstimados = r.LitrosEstimados,
                    Imagen = r.Imagen ?? string.Empty,
                    CostoProduccion = r.CostoProduccion,
                    TiempoVida = r.TiempoVida,
                    FechaRegistrado = r.FechaRegistrado,
                    Activo = r.Activo,
                    PrecioUnitarioBaseMayoreo = r.PrecioUnitarioBaseMayoreo,
                    PrecioUnitarioMinimoMayoreo = r.PrecioUnitarioMinimoMayoreo
                })
                .ToArrayAsync();

            return Ok(recetas);
        }

        // GET: api/recetas/5
        [AllowAnonymous]
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
                    PrecioPaquete1 = r.PrecioPaquete1,
                    PrecioPaquete12 = r.PrecioPaquete12,
                    PrecioPaquete6 = r.PrecioPaquete6,
                    PrecioPaquete24 = r.PrecioPaquete24,
                    TiempoVida = r.TiempoVida,
                    Especificaciones = r.Especificaciones ?? string.Empty,
                    RutaFondo = r.RutaFondo ?? string.Empty,
                    Puntuacion = r.Puntuacion,
                    Descripcion = r.Descripcion ?? string.Empty,
                    Nombre = r.Nombre,
                    CostoProduccion = r.CostoProduccion,
                    Imagen = r.Imagen,
                    Activo = r.Activo,
                    IngredientesReceta = r.IngredientesReceta.Select(ir => new IngredienteRecetaDto
                    {
                        Id = ir.IdInsumo,
                        Nombre = ir.Insumo.Nombre,
                        UnidadMedida = ir.Insumo.UnidadMedida,
                        Fijo = ir.Insumo.Fijo
                        Cantidad = ir.Cantidad,

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
                return NotFound(new { message = RecetaNoExisteMessage });
            }

            return Ok(receta);
        }

        // POST: api/recetas
        [HttpPost]
        public async Task<ActionResult<Receta>> PostReceta(RecetaInsertDto recetaDto)
        {
            Debug.WriteLine("##########################################################################");
            Debug.WriteLine("RecetaDto: " + recetaDto);
            Debug.WriteLine("##########################################################################");
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            //verificar que el insumo id = 1 exista en la receta a insertar
            if (!recetaDto.IngredientesReceta.Any(i => i.Id == 1))
            {
                return BadRequest(new { message = "La receta debe contener agua." });
            }

            //Verificar que se vaya a insertar por lo menos 80% de agua de los litros finales de la receta
            float litrosAgua = recetaDto.IngredientesReceta.Where(i => i.Id == 1).Select(i => i.Cantidad).FirstOrDefault();

            if (litrosAgua < recetaDto.LitrosEstimados * 0.8)
            {
                return BadRequest(new { message = "La receta debe contener al menos 80% de agua." });
            }

            //verificar que no exista una receta con el mismo nombre
            if (_context.Recetas.Any(r => r.Nombre == recetaDto.Nombre))
            {
                return BadRequest(new { message = "Ya existe una receta con el mismo nombre." });
            }

            //verificar que no haya agregado insumo id = 2 (botellas en la receta)
            if (recetaDto.IngredientesReceta.Any(i => i.Id == 2))
            {
                return BadRequest(new { message = "No se puede agregar botellas a la receta. Se calcularán automaticamente durante el envasado." });
            }


            var receta = new Receta
            {
                LitrosEstimados = recetaDto.LitrosEstimados,
                PrecioLitro = 0,
                Descripcion = recetaDto.Descripcion,
                Nombre = recetaDto.Nombre,
                Especificaciones = recetaDto.Especificaciones,
                PrecioPaquete1 = recetaDto.PrecioPaquete1,
                PrecioPaquete6 = recetaDto.PrecioPaquete6,
                PrecioPaquete12 = recetaDto.PrecioPaquete12,
                PrecioPaquete24 = recetaDto.PrecioPaquete24,
                CostoProduccion = 0,
                Imagen = recetaDto.Imagen,
                TiempoVida = recetaDto.TiempoVida,
                RutaFondo = recetaDto.RutaFondo,
                FechaRegistrado = System.DateTime.Now,
                Activo = true,
                IngredientesReceta = []
            };

            foreach (var ingredienteDto in recetaDto.IngredientesReceta)
            {
                var insumo = await _context.Insumos.FindAsync(ingredienteDto.Id);
                if (insumo == null)
                {
                    return NotFound(new { message = $"Insumo con ID {ingredienteDto.Id} no encontrado." });
                }

                var ingredienteReceta = new IngredienteReceta
                {
                    IdInsumo = ingredienteDto.Id,
                    Cantidad = ingredienteDto.Cantidad,
                    Receta = receta
                };

                receta.IngredientesReceta.Add(ingredienteReceta);
            }

            _context.Recetas.Add(receta);
            await _context.SaveChangesAsync();

            await CalcularCostoProduccion(receta.Id);

            try
            {
                var adminRoleId = await _context.Roles.Where(r => r.Name == AdminRoleName).Select(r => r.Id).FirstOrDefaultAsync();

                List<String> userIds = await _context.UserRoles.Where(ur => ur.RoleId == adminRoleId).Select(ur => ur.UserId).ToListAsync();

                foreach (var idAdmin in userIds)
                {
                    var notificacion = new Notificacion
                    {
                        IdUsuario = idAdmin,
                        Mensaje = $"Se agregó una nueva receta: {receta.Nombre}",
                        Fecha = DateTime.Now,
                        Tipo = 7,
                        Visto = false
                    };
                    _context.Notificaciones.Add(notificacion);
                }
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {

            }

            return Ok(new { message = "Receta insertada.", id = receta.Id });
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
            receta.PrecioPaquete1 = recetaDto.PrecioPaquete1;
            receta.PrecioPaquete6 = recetaDto.PrecioPaquete6;
            receta.PrecioPaquete12 = recetaDto.PrecioPaquete12;
            receta.PrecioPaquete24 = recetaDto.PrecioPaquete24;
            receta.Descripcion = recetaDto.Descripcion;
            receta.Especificaciones = recetaDto.Especificaciones;
            receta.Nombre = recetaDto.Nombre;
            receta.Imagen = recetaDto.Imagen;
            receta.TiempoVida = recetaDto.TiempoVida;
            receta.RutaFondo = recetaDto.RutaFondo;

            // Actualizar ingredientes
            var ingredientesExistentes = receta.IngredientesReceta.ToList();

            foreach (var ingredienteDto in recetaDto.IngredientesReceta)
            {
                var ingredienteExistente = ingredientesExistentes
                    .Find(i => i.IdInsumo == ingredienteDto.Id);

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
                        IdInsumo = ingredienteDto.Id,
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

            try
            {

                //encontrar la id del rol Admin
                var adminRoleId = _context.Roles.Where(r => r.Name == "Admin").Select(r => r.Id).FirstOrDefault();

                List<String> userIds = _context.UserRoles.Where(ur => ur.RoleId == adminRoleId).Select(ur => ur.UserId).ToList();

                foreach (var idAdmin in userIds)
                {
                    var notificacion = new Notificacion
                    {
                        IdUsuario = idAdmin,
                        Mensaje = $"Se modificó una receta: {receta.Nombre}",
                        Fecha = DateTime.Now,
                        Tipo = 7,
                        Visto = false
                    };
                    _context.Notificaciones.Add(notificacion);
                }
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {

            }
            return Ok(new { message = "Receta actualizada exitosamente." });
        }

        /* OBTENER PASOS RECETA */

        [HttpGet("{id}/pasos")]
        public async Task<ActionResult<IEnumerable<PasosRecetaDto>>> GetPasosReceta(int id)
        {
            var receta = await _context.Recetas
                .Include(r => r.PasosReceta)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (receta == null)
            {
                return NotFound(new { message = "Receta no existe." });
            }

            var pasos = receta.PasosReceta
                .Select(p => new PasosRecetaDto
                {
                    Id = p.Id,
                    Orden = p.Orden,
                    Descripcion = p.Descripcion,
                    Tiempo = p.Tiempo
                })
                .OrderBy(p => p.Orden)
                .ToList();

            return Ok(pasos);
        }

        /* CREAR PASOS RECETA */

        [HttpPost("{id}/pasos")]
        public async Task<ActionResult> CreatePasosInReceta(int id, List<PasosInsertDto> pasosDto)
        {
            var receta = await _context.Recetas
                .Include(r => r.PasosReceta)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (receta == null)
            {
                return NotFound(new { message = "Receta no existe." });
            }

            // Eliminar pasos existentes
            foreach (var pasoExistente in receta.PasosReceta.ToList())
            {
                _context.PasosRecetas.Remove(pasoExistente);
            }

            // Agregar nuevos pasos
            foreach (var pasoDto in pasosDto)
            {
                var nuevoPaso = new PasosReceta
                {
                    IdReceta = id,
                    Orden = pasoDto.Orden,
                    Tiempo = pasoDto.Tiempo,
                    Descripcion = pasoDto.Descripcion
                };
                receta.PasosReceta.Add(nuevoPaso);
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al crear los pasos.", error = ex.Message });
            }

            return Ok(new { message = "Pasos creados en la receta." });
        }


        [HttpPut("{id}/pasos")]
        public async Task<ActionResult> EditPasosInReceta(int id, List<PasosUpdateDto> pasosDto)
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
                if (!pasosDto.Any(p => p.Id == pasoExistente.Id))
                {
                    _context.PasosRecetas.Remove(pasoExistente);
                }
            }

            // Actualizar o agregar pasos
            foreach (var pasoDto in pasosDto)
            {
                var pasoExistente = pasosExistentes.FirstOrDefault(p => p.Id == pasoDto.Id);
                if (pasoExistente != null)
                {
                    // Actualizar paso existente
                    pasoExistente.Descripcion = pasoDto.Descripcion;
                    pasoExistente.Tiempo = pasoDto.Tiempo;
                    pasoExistente.Orden = pasoDto.Orden;
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

            try
            {

                //encontrar la id del rol Admin
                var adminRoleId = _context.Roles.Where(r => r.Name == "Admin").Select(r => r.Id).FirstOrDefault();

                List<String> userIds = _context.UserRoles.Where(ur => ur.RoleId == adminRoleId).Select(ur => ur.UserId).ToList();

                foreach (var idAdmin in userIds)
                {
                    var notificacion = new Notificacion
                    {
                        IdUsuario = idAdmin,
                        Mensaje = $"Se modificaron los pasos de la receta: {receta.Nombre}",
                        Fecha = DateTime.Now,
                        Tipo = 7,
                        Visto = false
                    };
                    _context.Notificaciones.Add(notificacion);
                }
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {

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
            try
            {

                //encontrar la id del rol Admin
                var adminRoleId = _context.Roles.Where(r => r.Name == "Admin").Select(r => r.Id).FirstOrDefault();

                List<String> userIds = _context.UserRoles.Where(ur => ur.RoleId == adminRoleId).Select(ur => ur.UserId).ToList();

                foreach (var idAdmin in userIds)
                {
                    var notificacion = new Notificacion
                    {
                        IdUsuario = idAdmin,
                        Mensaje = $"Se activo la receta: {receta.Nombre}",
                        Fecha = DateTime.Now,
                        Tipo = 7,
                        Visto = false
                    };
                    _context.Notificaciones.Add(notificacion);
                }
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {

            }
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

            try
            {

                //encontrar la id del rol Admin
                var adminRoleId = _context.Roles.Where(r => r.Name == "Admin").Select(r => r.Id).FirstOrDefault();

                List<String> userIds = _context.UserRoles.Where(ur => ur.RoleId == adminRoleId).Select(ur => ur.UserId).ToList();

                foreach (var idAdmin in userIds)
                {
                    var notificacion = new Notificacion
                    {
                        IdUsuario = idAdmin,
                        Mensaje = $"Se desactivó la receta: {receta.Nombre}",
                        Fecha = DateTime.Now,
                        Tipo = 7,
                        Visto = false
                    };
                    _context.Notificaciones.Add(notificacion);
                }
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {

            }

            return Ok(new { message = "Receta desactivada." });
        }

        //DELETE: api/recetas/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteReceta(int id)
        {
            var receta = await _context.Recetas
        .Include(r => r.IngredientesReceta)
        .Include(r => r.PasosReceta) // Incluir los pasos de la receta
        .FirstOrDefaultAsync(r => r.Id == id);

            if (receta == null)
            {
                return NotFound(new { message = "Receta no existe." });
            }

            /* ELIMINAMOS PRIMERO LOS INGREDIENTES */
            if (receta.IngredientesReceta != null && receta.IngredientesReceta.Any())
            {
                _context.IngredientesReceta.RemoveRange(receta.IngredientesReceta);
            }

            /* ELIMINAMOS LOS PASOS */
            if (receta.PasosReceta != null && receta.PasosReceta.Any())
            {
                _context.PasosRecetas.RemoveRange(receta.PasosReceta);
            }

            _context.Recetas.Remove(receta);
            await _context.SaveChangesAsync();

            try
            {

                //encontrar la id del rol Admin
                var adminRoleId = _context.Roles.Where(r => r.Name == "Admin").Select(r => r.Id).FirstOrDefault();

                List<String> userIds = _context.UserRoles.Where(ur => ur.RoleId == adminRoleId).Select(ur => ur.UserId).ToList();

                foreach (var idAdmin in userIds)
                {
                    var notificacion = new Notificacion
                    {
                        IdUsuario = idAdmin,
                        Mensaje = $"Se eliminó la receta: {receta.Nombre}",
                        Fecha = DateTime.Now,
                        Tipo = 7,
                        Visto = false
                    };
                    _context.Notificaciones.Add(notificacion);
                }
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {

            }
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

                if (nuevoCostoLitro > receta.PrecioLitro)
                {
                    receta.PrecioLitro = nuevoCostoLitro;
                }
                if (nuevoCostoLitro > receta.PrecioPaquete1)
                {
                    receta.PrecioPaquete1 = nuevoCostoLitro;
                }
                if (nuevoCostoLitro * 6 > receta.PrecioPaquete6)
                {
                    receta.PrecioPaquete6 = nuevoCostoLitro * 6;
                }
                if (nuevoCostoLitro * 12 > receta.PrecioPaquete12)
                {
                    receta.PrecioPaquete12 = nuevoCostoLitro * 12;
                }
                if (nuevoCostoLitro * 24 > receta.PrecioPaquete24)
                {
                    receta.PrecioPaquete24 = nuevoCostoLitro * 24;
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
            var productosCarousel = await _context.Recetas.Where(r => r.Activo == true)
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
            var productos = await _context.Recetas.Where(r => r.Activo == true).Select(receta => new
            {
                id = receta.Id,
                nombre = receta.Nombre,
                especificaciones = receta.Especificaciones,
                precioPaquete1 = receta.PrecioPaquete1,
                precioPaquete6 = receta.PrecioPaquete6,
                precioPaquete12 = receta.PrecioPaquete12,
                precioPaquete24 = receta.PrecioPaquete24,
                lotesMinimos = receta.LotesMinimos,
                lotesMaximos = receta.LotesMaximos,
                fechaRegistrado = receta.FechaRegistrado,
                tiempoVida = receta.TiempoVida,
                imagen = receta.Imagen,
                rutaFondo = receta.RutaFondo,
                cantidadEnStock = _context.Stocks
                        .Where(stock => stock.IdReceta == receta.Id)
                        .Sum(stock => stock.Cantidad)
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

            try
            {

                //encontrar la id del rol Admin
                var adminRoleId = _context.Roles.Where(r => r.Name == "Admin").Select(r => r.Id).FirstOrDefault();

                List<String> userIds = _context.UserRoles.Where(ur => ur.RoleId == adminRoleId).Select(ur => ur.UserId).ToList();

                foreach (var idAdmin in userIds)
                {
                    var notificacion = new Notificacion
                    {
                        IdUsuario = idAdmin,
                        Mensaje = $"Se actualizaron los precios de la receta: {receta.Nombre}",
                        Fecha = DateTime.Now,
                        Tipo = 7,
                        Visto = false
                    };
                    _context.Notificaciones.Add(notificacion);
                }
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {

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
