using CERVERICA.Data;
using CERVERICA.Dtos;
using CERVERICA.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

//[Authorize(Roles = "Admin")]
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
        var insumo = new Insumo
        {
            Nombre = insumoDto.Nombre,
            Descripcion = insumoDto.Descripcion,
            UnidadMedida = insumoDto.UnidadMedida,
            CantidadMaxima = insumoDto.CantidadMaxima,
            CantidadMinima = insumoDto.CantidadMinima,
            Merma = insumoDto.Merma,
            Activo = true
        };

        _context.Insumos.Add(insumo);
        await _context.SaveChangesAsync();

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

        insumo.Nombre = insumoDto.Nombre;
        insumo.Descripcion = insumoDto.Descripcion;
        insumo.UnidadMedida = insumoDto.UnidadMedida;
        insumo.CantidadMaxima = insumoDto.CantidadMaxima;
        insumo.CantidadMinima = insumoDto.CantidadMinima;
        insumo.Merma = insumoDto.Merma;

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
