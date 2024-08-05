using CERVERICA.Models;
using CERVERICA.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CERVERICA.Data;
using Microsoft.AspNetCore.Authorization;

namespace CERVERICA.Controllers
{
    //[Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ProveedoresController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ProveedoresController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/proveedores
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProveedoresDto>>> GetProveedores()
        {
            return Ok(await _context.Proveedores.ToListAsync());
        }

        // GET: api/proveedores/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ProveedoresDto>> GetProveedor(int id)
        {
            var proveedor = await _context.Proveedores.FindAsync(id);

            if (proveedor == null)
            {
                return NotFound(new { message = "Proveedor no existe." });
            }

            return Ok(proveedor);
        }

        // POST: api/proveedores
        [HttpPost]
        public async Task<ActionResult<ProveedorInsertDto>> PostProveedor(ProveedorInsertDto proveedorDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var proveedor = new Proveedor
            {
                Empresa = proveedorDto.Empresa,
                Direccion = proveedorDto.Direccion,
                Telefono = proveedorDto.Telefono,
                Email = proveedorDto.Email,
                NombreContacto = proveedorDto.NombreContacto,
                Activo = true
            };

            _context.Proveedores.Add(proveedor);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Proveedor insertado.", id = proveedor.Id });
        }

        // PUT: api/proveedores/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProveedor(int id, ProveedorInsertDto proveedorDto)
        {
            var proveedor = await _context.Proveedores.FindAsync(id);

            if (proveedor == null)
            {
                return NotFound(new { message = "Proveedor no existe." });
            }

            proveedor.Empresa = proveedorDto.Empresa;
            proveedor.Email = proveedorDto.Email;
            proveedor.Direccion = proveedorDto.Direccion;
            proveedor.Telefono = proveedorDto.Telefono;
            proveedor.NombreContacto = proveedorDto.NombreContacto;

            _context.Entry(proveedor).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProveedorExists(id))
                {
                    return NotFound(new { message = "Proveedor no existe." });
                }
                else
                {
                    throw;
                }
            }

            return Ok(new { message = "Proveedor actualizado."});
        }

        //Activar proveedor
        [HttpPost("activar/{id}")]
        public async Task<IActionResult> ActivarProveedor(int id)
        {
            var proveedor = await _context.Proveedores.FindAsync(id);

            if (proveedor == null)
            {
                return NotFound(new { message = "Proveedor no existe." });
            }

            proveedor.Activo = true;

            _context.Entry(proveedor).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProveedorExists(id))
                {
                    return NotFound(new { message = "Proveedor no existe." });
                }
                else
                {
                    throw;
                }
            }

            return Ok(new { message = "Proveedor activado." });
        }

        //Desactivar proveedor
        [HttpPost("desactivar/{id}")]
        public async Task<IActionResult> DesactivarProveedor(int id)
        {
            var proveedor = await _context.Proveedores.FindAsync(id);

            if (proveedor == null)
            {
                return NotFound(new { message = "Proveedor no existe." });
            }

            proveedor.Activo = false;

            _context.Entry(proveedor).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProveedorExists(id))
                {
                    return NotFound(new { message = "Proveedor no existe." });
                }
                else
                {
                    throw;
                }
            }

            return Ok(new { message = "Proveedor desactivado." });
        }

        // DELETE: api/proveedores/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProveedor(int id)
        {
            var proveedor = await _context.Proveedores.FindAsync(id);
            if (proveedor == null)
            {
                return NotFound(new { message = "Proveedor no existe." });
            }

            // Verificar si el proveedor está asociado a algún lote de insumo
            if (_context.LotesInsumos.Any(l => l.IdProveedor == id))
            {
                return BadRequest(new { message = "El proveedor ya asignó a un lote, no se puede borrar." });
            }

            _context.Proveedores.Remove(proveedor);
            await _context.SaveChangesAsync();

            return Ok(new {message = "Proveedor eliminado."});
        }

        private bool ProveedorExists(int id)
        {
            return _context.Proveedores.Any(e => e.Id == id);
        }
    }
}
