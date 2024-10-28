using CERVERICA.Models;
using CERVERICA.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CERVERICA.Data;
using Microsoft.AspNetCore.Authorization;
using CERVERICA.DTO.Usuarios;
using Microsoft.AspNetCore.Identity;

namespace CERVERICA.Controllers
{
    [Authorize(Roles = "Admin")]
    [ApiController]
    [Route("api/[controller]")]
    public class ClienteMayoristaController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _db;
        public ClienteMayoristaController(UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager,
        ApplicationDbContext context
        )
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;

        }

        // GET: api/clientes-mayoristas
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<ClienteMayorista>>> GetClientesMayoristas()
        {
            return Ok(await _context.ClientesMayoristas.ToListAsync());
        }

        // GET: api/clientes-mayoristas/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ClienteMayorista>> GetClienteMayorista(int id)
        {
            var clienteMayorista = await _context.ClientesMayoristas.FindAsync(id);

            if (clienteMayorista == null)
            {
                return NotFound(new { message = "Cliente mayorista no existe." });
            }

            return Ok(clienteMayorista);
        }

        // POST: api/clientes-mayorista
        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult<ClienteMayoristaDTO>> PostClienteMayorista(CrearUsuarioMayoristaDto clienteDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Paso 1: Registrar el usuario
            var user = new ApplicationUser
            {
                UserName = clienteDto.EmailContacto,
                Email = clienteDto.EmailContacto,
                FullName = clienteDto.NombreContacto,
                Activo = true
            };

            var result = await _userManager.CreateAsync(user, clienteDto.Password);

            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            var roleResult = await _userManager.AddToRoleAsync(user, clienteDto.Rol);
            if (!roleResult.Succeeded)
            {
                return BadRequest(roleResult.Errors);
            }

            // Paso 2: Seleccionar el agente de ventas con menos clientes asignados
            var agenteVentasRoleId = _context.Roles.Where(r => r.Name == "agenteventas").Select(r => r.Id).FirstOrDefault();

            var agentesVentas = _context.UserRoles
                .Where(ur => ur.RoleId == agenteVentasRoleId)
                .Select(ur => ur.UserId)
                .ToList();

            // Contar cuántos clientes tiene cada agente de ventas
            var agenteConMenosClientes = _context.ClientesMayoristas
                .GroupBy(cm => cm.IdAgenteVenta)
                .Where(g => agentesVentas.Contains(g.Key)) // Solo agentes con rol "agenteventas"
                .Select(g => new { IdAgenteVenta = g.Key, Count = g.Count() })
                .OrderBy(g => g.Count) // Ordenar por el que tiene menos clientes
                .FirstOrDefault();

            // Si no hay clientes asignados a ningún agente, asignar al primer agente disponible
            string agenteAsignadoId;
            if (agenteConMenosClientes == null)
            {
                agenteAsignadoId = agentesVentas.FirstOrDefault();
            }
            else
            {
                agenteAsignadoId = agenteConMenosClientes.IdAgenteVenta;
            }

            if (agenteAsignadoId == null)
            {
                return BadRequest("No se encontró ningún agente de ventas disponible.");
            }

            // Paso 3: Registrar el cliente mayorista
            var clienteMayorista = new ClienteMayorista
            {
                NombreEmpresa = clienteDto.NombreEmpresa,
                DireccionEmpresa = clienteDto.DireccionEmpresa,
                EmailEmpresa = clienteDto.EmailEmpresa,
                TelefonoEmpresa = clienteDto.TelefonoEmpresa,

                NombreContacto = clienteDto.NombreContacto,
                CargoContacto = clienteDto.CargoContacto,
                EmailContacto = clienteDto.EmailContacto,
                TelefonoContacto = clienteDto.TelefonoContacto,

                IdUsuario = user.Id, // Asignar el ID del usuario registrado
                IdAgenteVenta = agenteAsignadoId // Asignar el ID del agente de ventas con menos clientes
            };

            _context.ClientesMayoristas.Add(clienteMayorista);
            await _context.SaveChangesAsync();

            // Paso 4: Enviar notificaciones a los administradores
            try
            {
                var adminRoleId = _context.Roles.Where(r => r.Name == "Admin").Select(r => r.Id).FirstOrDefault();
                List<string> userIds = _context.UserRoles.Where(ur => ur.RoleId == adminRoleId).Select(ur => ur.UserId).ToList();

                foreach (var idAdmin in userIds)
                {
                    var notificacion = new Notificacion
                    {
                        IdUsuario = idAdmin,
                        Mensaje = $"Se agregó un nuevo cliente mayorista: {clienteMayorista.NombreEmpresa}",
                        Fecha = DateTime.Now,
                        Tipo = 6,
                        Visto = false
                    };
                    _context.Notificaciones.Add(notificacion);
                }
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // Manejo de excepción opcional
            }

            return Ok(new { message = "Cliente mayorista insertado.", id = clienteMayorista.Id });
        }


        /*
        // POST: api/clientes-mayorista
        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult<ClienteMayoristaDTO>> PostClienteMayorista(CrearUsuarioMayoristaDto clienteDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Paso 1: Registrar el usuario
            var user = new ApplicationUser
            {
                UserName = clienteDto.EmailContacto,
                Email = clienteDto.EmailContacto,
                FullName = clienteDto.NombreContacto,
                Activo = true
            };

            var result = await _userManager.CreateAsync(user, clienteDto.Password);

            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            var roleResult = await _userManager.AddToRoleAsync(user, clienteDto.Rol);
            if (!roleResult.Succeeded)
            {
                return BadRequest(roleResult.Errors);
            }

            // Paso 2: Registrar el cliente mayorista
            var clienteMayorista = new ClienteMayorista
            {
                NombreEmpresa = clienteDto.NombreEmpresa,
                DireccionEmpresa = clienteDto.DireccionEmpresa,
                EmailEmpresa = clienteDto.EmailEmpresa,
                TelefonoEmpresa = clienteDto.TelefonoEmpresa,


                NombreContacto = clienteDto.NombreContacto,
                CargoContacto = clienteDto.CargoContacto,
                EmailContacto = clienteDto.EmailContacto,
                TelefonoContacto = clienteDto.TelefonoContacto,

                UserId = user.Id, // Asignar el ID del usuario registrado
                AgenteVentaId = user.Id
            };

            _context.ClientesMayoristas.Add(clienteMayorista);
            await _context.SaveChangesAsync();

            // Paso 3: Enviar notificaciones a los administradores
            try
            {
                var adminRoleId = _context.Roles.Where(r => r.Name == "Admin").Select(r => r.Id).FirstOrDefault();
                List<string> userIds = _context.UserRoles.Where(ur => ur.RoleId == adminRoleId).Select(ur => ur.UserId).ToList();

                foreach (var idAdmin in userIds)
                {
                    var notificacion = new Notificacion
                    {
                        IdUsuario = idAdmin,
                        Mensaje = $"Se agregó un nuevo cliente mayorista: {clienteMayorista.NombreEmpresa}",
                        Fecha = DateTime.Now,
                        Tipo = 6,
                        Visto = false
                    };
                    _context.Notificaciones.Add(notificacion);
                }
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // Manejo de excepción opcional
            }

            return Ok(new { message = "Cliente mayorista insertado.", id = clienteMayorista.Id });
        }
        */

        private bool ClienteMayoristaExists(int id)
        {
            return _context.ClientesMayoristas.Any(e => e.Id == id);
        }
    }
}
