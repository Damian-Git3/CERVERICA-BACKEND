using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using CERVERICA.Models;
using CERVERICA.Data;
using CERVERICA.Dtos;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace CERVERICA.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize(Roles = "Admin,Vendedor")]
    public class LotesInsumosController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public LotesInsumosController(ApplicationDbContext context)
        {
            _context = context;
        }

        private string GetCurrentUserId()
        {
            return User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "a0a0aaa0-0000-0aa0-0000-00aaa0aa0a00";
        }

        // GET: api/LotesInsumos
        [HttpGet]
        public async Task<ActionResult<IEnumerable<LoteInsumoDto>>> GetLotesInsumos()
        {
            var lotesInsumos = await _context.LotesInsumos.Where(l => l.Cantidad > 0 && System.DateTime.Now < l.FechaCaducidad)
                .Select(l => new LoteInsumoDto
                {
                    Id = l.Id,
                    IdProveedor = l.IdProveedor,
                    IdInsumo = l.IdInsumo,
                    IdUsuario = l.IdUsuario,
                    FechaCaducidad = l.FechaCaducidad,
                    Cantidad = l.Cantidad,
                    Caducado = l.Caducado,
                    FechaCompra = l.FechaCompra,
                    PrecioUnidad = l.PrecioUnidad,
                    MontoCompra = l.MontoCompra,
                    Merma = l.Merma
                })
                .ToListAsync();

            return Ok(lotesInsumos);
        }

        [HttpGet("todos")]
        public async Task<ActionResult<IEnumerable<LoteInsumoDto>>> GetLotesInsumosTodos()
        {
            var lotesInsumos = await _context.LotesInsumos
                .Select(l => new LoteInsumoDto
                {
                    Id = l.Id,
                    IdProveedor = l.IdProveedor,
                    IdInsumo = l.IdInsumo,
                    IdUsuario = l.IdUsuario,
                    FechaCaducidad = l.FechaCaducidad,
                    Cantidad = l.Cantidad,
                    Caducado = l.Caducado,
                    FechaCompra = l.FechaCompra,
                    PrecioUnidad = l.PrecioUnidad,
                    MontoCompra = l.MontoCompra,
                    Merma = l.Merma
                })
                .ToListAsync();

            return Ok(lotesInsumos);
        }

        [HttpGet("vacios")]
        public async Task<ActionResult<IEnumerable<LoteInsumoDto>>> GetLotesInsumosVacios()
        {
            var lotesInsumos = await _context.LotesInsumos.Where(l => l.Cantidad == 0)
                .Select(l => new LoteInsumoDto
                {
                    Id = l.Id,
                    IdProveedor = l.IdProveedor,
                    IdInsumo = l.IdInsumo,
                    IdUsuario = l.IdUsuario,
                    FechaCaducidad = l.FechaCaducidad,
                    Cantidad = l.Cantidad,
                    Caducado = l.Caducado,
                    FechaCompra = l.FechaCompra,
                    PrecioUnidad = l.PrecioUnidad,
                    MontoCompra = l.MontoCompra,
                    Merma = l.Merma
                })
                .ToListAsync();

            return Ok(lotesInsumos);
        }

        [HttpGet("caducados")]
        public async Task<ActionResult<IEnumerable<LoteInsumoDto>>> GetLotesInsumosCaducados()
        {
            var lotesInsumos = await _context.LotesInsumos.Where(l => l.Caducado > 0)
                .Select(l => new LoteInsumoDto
                {
                    Id = l.Id,
                    IdProveedor = l.IdProveedor,
                    IdInsumo = l.IdInsumo,
                    IdUsuario = l.IdUsuario,
                    FechaCaducidad = l.FechaCaducidad,
                    Cantidad = l.Cantidad,
                    Caducado = l.Caducado,
                    FechaCompra = l.FechaCompra,
                    PrecioUnidad = l.PrecioUnidad,
                    MontoCompra = l.MontoCompra,
                    Merma = l.Merma

                })
                .ToListAsync();

            return Ok(lotesInsumos);
        }

        // GET: api/LotesInsumos/5
        [HttpGet("{id}")]
        public async Task<ActionResult<LoteInsumoDto>> GetLoteInsumo(int id)
        {
            var loteInsumo = await _context.LotesInsumos
                .Where(l => l.Id == id)
                .Select(l => new LoteInsumoDto
                {
                    Id = l.Id,
                    IdProveedor = l.IdProveedor,
                    IdInsumo = l.IdInsumo,
                    IdUsuario = l.IdUsuario,
                    FechaCaducidad = l.FechaCaducidad,
                    Cantidad = l.Cantidad,
                    Caducado = l.Caducado,
                    FechaCompra = l.FechaCompra,
                    PrecioUnidad = l.PrecioUnidad,
                    MontoCompra = l.MontoCompra,
                    Merma = l.Merma
                })
                .FirstOrDefaultAsync();

            if (loteInsumo == null)
            {
                return NotFound(new { message = "Lote de Insumo no existe." });
            }
            if (loteInsumo.Cantidad == 0)
            {
                return Ok(new { message = "Lote de Insumo vacío.", loteInsumo});
            }
            if (System.DateTime.Now > loteInsumo.FechaCaducidad)
            {
                return Ok(new { message = "Lote de Insumo caducado.", loteInsumo });
            }

            return Ok(loteInsumo);
        }

        // GET: api/LotesInsumos/insumo/5
        [HttpGet("insumo/{IdInsumo}")]
        public async Task<ActionResult<LoteInsumoDto>> GetLoteInsumoPorInsumo(int IdInsumo)
        {
            var loteInsumo = await _context.LotesInsumos
                .Where(l => l.IdInsumo == IdInsumo && l.Cantidad> 0 && System.DateTime.Now < l.FechaCaducidad)
                .Select(l => new LoteInsumo
                {
                    Id = l.Id,
                    IdProveedor = l.IdProveedor,
                    IdInsumo = l.IdInsumo,
                    IdUsuario = l.IdUsuario,
                    FechaCaducidad = l.FechaCaducidad,
                    Cantidad = l.Cantidad,
                    Caducado = l.Caducado,
                    FechaCompra = l.FechaCompra,
                    PrecioUnidad = l.PrecioUnidad,
                    MontoCompra = l.MontoCompra,
                    Merma = l.Merma
                })
                .ToListAsync();

            if (loteInsumo == null)
            {
                return NotFound(new { message = "Lote de Insumo no existe." });
            }

            return Ok(loteInsumo);
        }

        // GET: api/LotesInsumos/fechaCompra
        [HttpGet("fechaCompra")]
        public async Task<ActionResult<LoteInsumoDto>> GetLoteInsumoPorFecha(DateTime fecha1, DateTime fecha2)
        {

            //consulta para obtener los lotes de insumos que se compraron entre las fechas indicadas
            var lotesInsumos = await _context.LotesInsumos
                .Where(l => l.FechaCompra >= fecha1 && l.FechaCompra <= fecha2 && l.Cantidad>0)
                .Select(l => new LoteInsumoDto
                {
                    Id = l.Id,
                    IdProveedor = l.IdProveedor,
                    IdInsumo = l.IdInsumo,
                    IdUsuario = l.IdUsuario,
                    FechaCaducidad = l.FechaCaducidad,
                    Cantidad = l.Cantidad,
                    Caducado = l.Caducado,
                    FechaCompra = l.FechaCompra,
                    PrecioUnidad = l.PrecioUnidad,
                    MontoCompra = l.MontoCompra,
                    Merma = l.Merma
                })
                .ToListAsync();


            if (lotesInsumos == null)
            {
                return NotFound(new { message = "Lote de Insumo no existe." });
            }

            return Ok(lotesInsumos);
        }

        // POST: api/LotesInsumos
        [HttpPost]
        public async Task<ActionResult<LoteInsumoDto>> PostLoteInsumo(LoteInsumoInsertDto loteInsumoDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var precioUnidad = loteInsumoDto.MontoCompra / loteInsumoDto.Cantidad;

            //verificar que el insumo exista
            var insumo = await _context.Insumos.FindAsync(loteInsumoDto.IdInsumo);
            if (insumo == null)
            {
                return NotFound(new { message = "Insumo no existe." });
            }

            //verificar que el proveedor exista
            var proveedor = await _context.Proveedores.FindAsync(loteInsumoDto.IdProveedor);
            if (proveedor == null)
            {
                return NotFound(new { message = "Proveedor no existe." });
            }

            var loteInsumo = new LoteInsumo
            {
                IdProveedor = loteInsumoDto.IdProveedor,
                IdInsumo = loteInsumoDto.IdInsumo,
                IdUsuario = GetCurrentUserId(),
                FechaCaducidad = loteInsumoDto.FechaCaducidad,
                Cantidad = loteInsumoDto.Cantidad,
                Caducado = 0,
                FechaCompra = System.DateTime.Now,
                PrecioUnidad = precioUnidad,
                MontoCompra = loteInsumoDto.MontoCompra,
                Merma = 0
            };

            _context.LotesInsumos.Add(loteInsumo);
            await _context.SaveChangesAsync();

            await RecalcularCostoUnitario(loteInsumo.IdInsumo);

            try
            { //##########  Enviar notificación a los Admins ##########

                //encontrar la id del rol Admin
                var adminRoleId = _context.Roles.Where(r => r.Name == "Admin").Select(r => r.Id).FirstOrDefault();

                List<String> userIds = _context.UserRoles.Where(ur => ur.RoleId == adminRoleId).Select(ur => ur.UserId).ToList();

                foreach (var idAdmin in userIds)
                {
                    var notificacion = new Notificacion
                    {
                        IdUsuario = idAdmin,
                        Mensaje = $"Se ha comprado un nuevo lote del insumo: {insumo.Nombre}",
                        Fecha = DateTime.Now,
                        Tipo = 4,
                        Visto = false
                    };
                    _context.Notificaciones.Add(notificacion);
                }
                await _context.SaveChangesAsync();
            }
            catch (Exception ex){}

            return Ok(new { message = "Lote de Insumo insertado.", id = loteInsumo.Id });
        }

        // PUT: api/lotesinsumos/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutLoteInsumo(int id, LoteInsumoEditarDto loteInsumoDto)
        {

            var loteInsumo = await _context.LotesInsumos.FindAsync(id);

            if (loteInsumo == null)
            {
                return NotFound(new { message = "LoteInsumo no existe." });
            }

            //verificar que no hayan pasado mas de 10 minutos desde la compra
            if (System.DateTime.Now.Subtract(loteInsumo.FechaCompra).TotalMinutes > 10)
            {
                return BadRequest(new { message = "No se puede modificar el lote de insumo porque han pasado mas de 10 minutos desde la compra." });
            }

            var precioUnidad = loteInsumoDto.MontoCompra / loteInsumoDto.Cantidad;

            loteInsumo.IdUsuario = GetCurrentUserId();
            loteInsumo.FechaCaducidad = loteInsumoDto.FechaCaducidad;
            loteInsumo.Cantidad = loteInsumoDto.Cantidad;
            loteInsumo.PrecioUnidad = precioUnidad;
            loteInsumo.MontoCompra = loteInsumoDto.MontoCompra;

            _context.Entry(loteInsumo).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                await RecalcularCostoUnitario(loteInsumo.IdInsumo);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LoteInsumoExists(id))
                {
                    return NotFound(new { message = "Lote Insumo no existe." });
                }
                else
                {
                    throw;
                }
            }

            try
            { //##########  Enviar notificación a los Admins ##########

                //encontrar la id del rol Admin
                var adminRoleId = _context.Roles.Where(r => r.Name == "Admin").Select(r => r.Id).FirstOrDefault();

                List<String> userIds = _context.UserRoles.Where(ur => ur.RoleId == adminRoleId).Select(ur => ur.UserId).ToList();

                //encontrar el insumo
                var insumo = await _context.Insumos.FindAsync(loteInsumo.IdInsumo);

                foreach (var idAdmin in userIds)
                {
                    var notificacion = new Notificacion
                    {
                        IdUsuario = idAdmin,
                        Mensaje = $"Se ha modificado la compra de un nuevo lote del insumo: {insumo.Nombre}",
                        Fecha = DateTime.Now,
                        Tipo = 4,
                        Visto = false
                    };
                    _context.Notificaciones.Add(notificacion);
                }
                await _context.SaveChangesAsync();
            }
            catch (Exception ex) { }

            return Ok(new { message = "Lote de Insumo actualizado." });
        }

        // POST: api/lotesinsumos/merma/5
        [HttpPost("merma/{id}")]
        public async Task<IActionResult> MermaLoteInsumo(int id, float merma)
        {
            var loteInsumo = await _context.LotesInsumos.FindAsync(id);

            if (loteInsumo == null)
            {
                return NotFound(new { message = "LoteInsumo no existe." });
            }

            if (merma > loteInsumo.Cantidad)
            {
                return BadRequest(new { message = "La merma no puede ser mayor a la cantidad del lote." });
            }

            float nuevaCantidad = loteInsumo.Cantidad - merma;

            if (nuevaCantidad == 0)
            {
                var otroLote = await _context.LotesInsumos
                    .Where(l => l.IdInsumo == loteInsumo.IdInsumo && l.Id != id && l.Cantidad>0)
                    .FirstOrDefaultAsync();

                if (otroLote == null)
                {
                    loteInsumo.PrecioUnidad = (loteInsumo.PrecioUnidad * loteInsumo.Cantidad) / 1;

                    loteInsumo.Cantidad = nuevaCantidad;
                    loteInsumo.Merma += merma;

                    _context.Entry(loteInsumo).State = EntityState.Modified;
                } else
                {
                    float nuevoPrecioUnidad = (otroLote.PrecioUnidad * otroLote.Cantidad + loteInsumo.PrecioUnidad * loteInsumo.Cantidad) / otroLote.Cantidad;

                    otroLote.PrecioUnidad = nuevoPrecioUnidad;

                    _context.Entry(otroLote).State = EntityState.Modified;
                }
            }
            else
            {
                loteInsumo.PrecioUnidad = (loteInsumo.PrecioUnidad * loteInsumo.Cantidad) / nuevaCantidad;

                loteInsumo.Cantidad = nuevaCantidad;
                loteInsumo.Merma += merma;

                _context.Entry(loteInsumo).State = EntityState.Modified;
            }

            try
            {
                await _context.SaveChangesAsync();
                await RecalcularCostoUnitario(loteInsumo.IdInsumo);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LoteInsumoExists(id))
                {
                    return NotFound(new { message = "Lote Insumo no existe." });
                }
                else
                {
                    throw;
                }
            }

            try
            { //##########  Enviar notificación a los Admins ##########

                //encontrar la id del rol Admin
                var adminRoleId = _context.Roles.Where(r => r.Name == "Admin").Select(r => r.Id).FirstOrDefault();

                List<String> userIds = _context.UserRoles.Where(ur => ur.RoleId == adminRoleId).Select(ur => ur.UserId).ToList();
                //encontrar el insumo
                var insumo = await _context.Insumos.FindAsync(loteInsumo.IdInsumo);
                foreach (var idAdmin in userIds)
                {
                    var notificacion = new Notificacion
                    {
                        IdUsuario = idAdmin,
                        Mensaje = $"Se ha registrado merma de un lote del insumo: {insumo.Nombre}",
                        Fecha = DateTime.Now,
                        Tipo = 4,
                        Visto = false
                    };
                    _context.Notificaciones.Add(notificacion);
                }
                await _context.SaveChangesAsync();
            }
            catch (Exception ex) { }

            return Ok(new { message = "Cantidad del Lote de Insumo y Precio Unidad actualizados." });
        }

        
        private bool LoteInsumoExists(int id)
        {
            return _context.LotesInsumos.Any(e => e.Id == id);
        }

        private async Task RecalcularCostoUnitario(int idInsumo)
        {
            var insumo = await _context.Insumos.Include(i => i.LotesInsumos).FirstOrDefaultAsync(i => i.Id == idInsumo);
            if (insumo != null && insumo.LotesInsumos != null && insumo.LotesInsumos.Any())
            {
                var sumaPreciosPorCantidad = insumo.LotesInsumos.Sum(l => l.PrecioUnidad * l.Cantidad);
                var sumaCantidades = insumo.LotesInsumos.Sum(l => l.Cantidad);

                if (sumaCantidades == 0)
                {
                    insumo.CostoUnitario = 0; // O algún valor por defecto
                }
                else
                {
                    insumo.CostoUnitario = (float)(sumaPreciosPorCantidad / sumaCantidades);
                }

                _context.Entry(insumo).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                // Recalcular el costo de producción de todas las recetas que usan este insumo
                var recetasConInsumo = await _context.Recetas
                    .Include(r => r.IngredientesReceta)
                    .Where(r => r.IngredientesReceta.Any(ir => ir.IdInsumo == idInsumo))
                    .ToListAsync();

                foreach (var receta in recetasConInsumo)
                {
                    await RecalcularCostoProduccion(receta.Id);
                }
            }
        }

        //calcular costo de produccion
        private async Task RecalcularCostoProduccion(int idReceta)
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
    }
}
