using CERVERICA.Data;
using CERVERICA.Dtos;
using CERVERICA.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

[Authorize(Roles = "Admin")]
[Route("api/[controller]")]
[ApiController]
public class InsumosController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public InsumosController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<InsumosCantidadDto>>> GetInsumos()
    {
        return await _context.Insumos
            .Select(insumo => new InsumosCantidadDto
            {
                Id = insumo.Id,
                Nombre = insumo.Nombre,
                Descripcion = insumo.Descripcion,
                UnidadMedida = insumo.UnidadMedida,
                CantidadMaxima = insumo.CantidadMaxima,
                CantidadMinima = insumo.CantidadMinima,
                CostoUnitario = insumo.CostoUnitario,
                Merma = insumo.Merma,
                Activo = insumo.Activo,
                Fijo = insumo.Fijo,
                CantidadTotalLotes = _context.LotesInsumos
                    .Where(lote => lote.IdInsumo == insumo.Id)
                    .Sum(lote => (float?)lote.Cantidad) ?? 0  // Nueva lógica para sumar las cantidades
            }).ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<InsumosCantidadDto>> GetInsumo(int id)
    {
        var insumo = await _context.Insumos.FindAsync(id);

        if (insumo == null)
        {
            return NotFound();
        }

        return new InsumosCantidadDto
        {
            Id = insumo.Id,
            Nombre = insumo.Nombre,
            Descripcion = insumo.Descripcion,
            UnidadMedida = insumo.UnidadMedida,
            CantidadMaxima = insumo.CantidadMaxima,
            CantidadMinima = insumo.CantidadMinima,
            CostoUnitario = insumo.CostoUnitario,
            Merma = insumo.Merma,
            Activo = insumo.Activo,
            CantidadTotalLotes = _context.LotesInsumos
                    .Where(lote => lote.IdInsumo == insumo.Id)
                    .Sum(lote => (float?)lote.Cantidad) ?? 0
        };
    }

    [HttpPost]
    public async Task<ActionResult<InsumoInsertDto>> PostInsumo(InsumoInsertDto insumoDto)
    {

        //verificar que no existe un insumo con el mismo nombre
        if (_context.Insumos.Any(i => i.Nombre == insumoDto.Nombre))
        {
            return BadRequest(new { message = "Ya existe un insumo con el mismo nombre." });
        }

        var insumo = new Insumo
        {
            Nombre = insumoDto.Nombre,
            Descripcion = insumoDto.Descripcion,
            UnidadMedida = insumoDto.UnidadMedida,
            CantidadMaxima = insumoDto.CantidadMaxima ?? 0,
            CantidadMinima = insumoDto.CantidadMinima ?? 0,
            Merma = insumoDto.Merma ?? 0,
            Fijo = insumoDto.Fijo,
            Activo = true
        };

        _context.Insumos.Add(insumo);
        await _context.SaveChangesAsync();

        try
        {

            //encontrar la id del rol Admin
            var adminRoleId = _context.Roles.Where(r => r.Name == "Admin").Select(r => r.Id).FirstOrDefault();

            List<String> userIds = _context.UserRoles.Where(ur => ur.RoleId == adminRoleId).Select(ur => ur.UserId).ToList();

            foreach (var id in userIds)
            {
                var notificacion = new Notificacion
                {
                    IdUsuario = id,
                    Mensaje = $"Se ha agregado un nuevo insumo: {insumo.Nombre}",
                    Fecha = DateTime.Now,
                    Tipo = 3,
                    Visto = false
                };
                _context.Notificaciones.Add(notificacion);
            }
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {

        }

        return Ok(new { message = "Insumo insertado.", id = insumo.Id });
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> PutInsumo(int id, InsumoInsertDto insumoDto)
    {
        var insumo = await _context.Insumos.FindAsync(id);
        if (insumo == null)
        {
            return NotFound();
        }

        //verificar que no existe un insumo con el mismo nombre
        if (_context.Insumos.Any(i => i.Nombre == insumoDto.Nombre && i.Id != id))
        {
            return BadRequest(new { message = "Ya existe un insumo con el mismo nombre." });
        }

        insumo.Nombre = insumoDto.Nombre;
        insumo.Descripcion = insumoDto.Descripcion;
        insumo.UnidadMedida = insumoDto.UnidadMedida;
        insumo.CantidadMaxima = insumoDto.CantidadMaxima ?? 0;
        insumo.CantidadMinima = insumoDto.CantidadMinima ?? 0;
        insumo.Merma = insumoDto.Merma ?? 0;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!InsumoExists(id))
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
                    Mensaje = $"Se ha modificado el insumo: {insumo.Nombre}",
                    Fecha = DateTime.Now,
                    Tipo = 3,
                    Visto = false
                };
                _context.Notificaciones.Add(notificacion);
            }
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {

        }

        return Ok(new { message = "Insumo actualizado." });
    }

    // POST: api/insumos/activar/{id}

    [HttpPost("activar/{id}")]
    public async Task<IActionResult> ActivarInsumo(int id)
    {
        var insumo = await _context.Insumos.FindAsync(id);
        if (insumo == null)
        {
            return NotFound(new { message = "Insumo no encontrado." });
        }

        insumo.Activo = true;
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
                    Mensaje = $"Se ha activado otra vez el insumo: {insumo.Nombre}",
                    Fecha = DateTime.Now,
                    Tipo = 3,
                    Visto = false
                };
                _context.Notificaciones.Add(notificacion);
            }
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {

        }

        return Ok(new { message = "Insumo activado." });
    }

    // POST: api/insumos/desactivar/{id}

    [HttpPost("desactivar/{id}")]
    public async Task<IActionResult> DesactivarInsumo(int id)
    {
        var insumo = await _context.Insumos.FindAsync(id);
        if (insumo == null)
        {
            return NotFound(new { message = "Insumo no encontrado." });
        }

        insumo.Activo = false;
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
                    Mensaje = $"Se ha desactivado el insumo: {insumo.Nombre}",
                    Fecha = DateTime.Now,
                    Tipo = 3,
                    Visto = false
                };
                _context.Notificaciones.Add(notificacion);
            }
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        { }

        return Ok(new { message = "Insumo desactivado." });
    }

    // DELETE: api/insumos/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteInsumo(int id)
    {
        var insumo = await _context.Insumos.FindAsync(id);
        if (insumo == null)
        {
            return NotFound(new { message = "Insumo no encontrado." });
        }

        try
        {
            _context.Insumos.Remove(insumo);
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
                        Mensaje = $"Se ha borrado del sistema el insumo: {insumo.Nombre}",
                        Fecha = DateTime.Now,
                        Tipo = 3,
                        Visto = false
                    };
                    _context.Notificaciones.Add(notificacion);
                }
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {

            }

            return Ok(new { message = "Insumo eliminado." });
        }
        catch (DbUpdateException ex) when (ex.InnerException is SqlException sqlEx && sqlEx.Number == 547)
        {
            // Error de clave foránea (restricción referencial)
            return BadRequest(new { message = "El insumo ya se esta siendo ocupado, no se puede borrar." });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Surgió un error al borrar el insumo.", details = ex.Message });
        }
    }

    private bool InsumoExists(int id)
    {
        return _context.Insumos.Any(e => e.Id == id);
    }
}
