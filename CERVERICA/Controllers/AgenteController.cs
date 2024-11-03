using CERVERICA.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CERVERICA.Data;
using CERVERICA.DTO.Agente;

namespace CERVERICA.Controllers
{

    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class AgenteController : ControllerBase
    {

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;


        public AgenteController(UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager,
        ApplicationDbContext context,
        IConfiguration configuration
        )
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
            _configuration = configuration;

        }

        // GET: api/agentes
        // GET: api/agentes
        [HttpGet("agentes")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<ApplicationUser>>> GetAgentes()
        {
            // Obtener el ID del rol "Agente"
            var agenteRoleId = await _context.Roles
                .Where(r => r.Name == "Agente")
                .Select(r => r.Id)
                .FirstOrDefaultAsync();

            if (agenteRoleId == null)
            {
                return NotFound(new { message = "No se encontró el rol 'Agente'." });
            }

            // Obtener los usuarios con el rol "Agente"
            var agentes = await _context.UserRoles
                .Where(ur => ur.RoleId == agenteRoleId)
                .Select(ur => ur.UserId)
                .ToListAsync();

            // Obtener los detalles de los usuarios
            var usuariosAgentes = await _context.Users
                .Where(u => agentes.Contains(u.Id))
                .ToListAsync();

            return Ok(usuariosAgentes);
        }


        // POST: api/agente/solicitud-cambio-agente
        [HttpPost("solicitud-cambio-agente")]
        [AllowAnonymous]
        public async Task<ActionResult> CreateSolicitudCambioAgente([FromBody] SolicitudCambioAgenteDTO solicitudDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var solicitud = new SolicitudesCambioAgente
            {
                IdAgenteVentaActual = solicitudDto.IdAgenteVentaActual,
                FechaSolicitud = solicitudDto.FechaSolicitud,
                Motivo = solicitudDto.Motivo,
                Solicitante = solicitudDto.Solicitante,
                IdMayorista = solicitudDto.IdMayorista,
                Estatus = "Pendiente"
            };

            _context.SolicitudesCambioAgente.Add(solicitud);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetSolicitudCambioAgente), new { id = solicitud.Id }, solicitud);
        }

        // PUT: api/agente/solicitud-cambio-agente/{id}
        [HttpPut("solicitud-cambio-agente/{id}")]
        [AllowAnonymous]
        public async Task<ActionResult> UpdateSolicitudCambioAgente(int id, [FromBody] ActualizarSolicitudCambioAgenteDTO solicitudDto)
        {
            if (id != solicitudDto.Id)
            {
                return BadRequest(new { message = "El ID proporcionado no coincide con la solicitud a actualizar." });
            }

            var solicitudExistente = await _context.SolicitudesCambioAgente.FindAsync(id);
            if (solicitudExistente == null)
            {
                return NotFound(new { message = "La solicitud de cambio de agente no existe." });
            }

            // Verificar si IdAgenteNuevo no se ha enviado o es nulo
            if (string.IsNullOrEmpty(solicitudDto.IdAgenteNuevo))
            {
                // Paso 2: Seleccionar el agente de ventas con menos clientes asignados
                var agenteVentasRoleId = _context.Roles
                    .Where(r => r.Name == "Agente")
                    .Select(r => r.Id)
                    .FirstOrDefault();

                var agentesVentas = _context.UserRoles
                    .Where(ur => ur.RoleId == agenteVentasRoleId)
                    .Select(ur => ur.UserId)
                    .ToList();

                string agenteAsignadoId = null;

                // Verificar si el estatus es "Rechazada" y evitar la asignación del nuevo agente
                if (solicitudDto.Estatus != "Rechazada")
                {
                    // Intentar encontrar un nuevo agente
                    bool agenteEncontrado = false;
                    while (!agenteEncontrado)
                    {
                        var agenteConMenosClientes = _context.ClientesMayoristas
                            .GroupBy(cm => cm.IdAgenteVenta)
                            .Where(g => agentesVentas.Contains(g.Key))
                            .Select(g => new { IdAgenteVenta = g.Key, Count = g.Count() })
                            .OrderBy(g => g.Count)
                            .FirstOrDefault();

                        if (agenteConMenosClientes == null)
                        {
                            agenteAsignadoId = agentesVentas.FirstOrDefault();
                            agenteEncontrado = true; // Si no hay más agentes, asignar el primero
                        }
                        else
                        {
                            agenteAsignadoId = agenteConMenosClientes.IdAgenteVenta;

                            // Validar que el agente asignado no sea el mismo que el agente actual
                            if (agenteAsignadoId != solicitudDto.IdAgenteActual)
                            {
                                agenteEncontrado = true; // Se encontró un agente válido
                            }
                            else
                            {
                                // Si es el mismo, eliminarlo de la lista de agentes y buscar el siguiente
                                agentesVentas.Remove(agenteAsignadoId);
                            }
                        }
                    }
                }

                // Actualizar la solicitud con el nuevo agente de ventas y fecha de respuesta
                if (agenteAsignadoId != null)
                {
                    solicitudExistente.IdAgenteVentaNuevo = agenteAsignadoId; // Asigna el nuevo agente
                }
            }
            else
            {
                // Si se proporciona IdAgenteNuevo, se asigna directamente
                solicitudExistente.IdAgenteVentaNuevo = solicitudDto.IdAgenteNuevo;
            }

            solicitudExistente.FechaRespuesta = solicitudDto.FechaRespuesta;
            solicitudExistente.Estatus = solicitudDto.Estatus;
            solicitudExistente.IdAdministrador = solicitudDto.IdAdministrador;
            solicitudExistente.MotivoRechazo = solicitudDto.MotivoRechazo;


            try
            {
                await _context.SaveChangesAsync();

                // Paso 3: Asignar IdAgenteVentaNuevo a IdAgenteVenta del ClienteMayorista
                var clienteMayorista = await _context.ClientesMayoristas
                    .FindAsync(solicitudDto.IdMayorista); // Busca el cliente mayorista por ID

                if (clienteMayorista != null && solicitudExistente.IdAgenteVentaNuevo != null)
                {
                    clienteMayorista.IdAgenteVenta = solicitudExistente.IdAgenteVentaNuevo; // Asigna el nuevo IdAgenteVenta
                    await _context.SaveChangesAsync(); // Guarda los cambios
                }

                return Ok(new { message = "Success" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al actualizar la solicitud de cambio de agente.", error = ex.Message });
            }
        }

        // GET: api/agente/solicitud-cambio-agente/5
        [HttpGet("solicitud-cambio-agente/{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<SolicitudesCambioAgente>> GetSolicitudCambioAgente(int id)
        {
            var solicitud = await _context.SolicitudesCambioAgente
                .Include(s => s.AgenteVentaActual)
                .Include(s => s.AgenteVentaNuevo)
                .Include(s => s.Mayorista)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (solicitud == null)
            {
                return NotFound(new { message = "Solicitud de cambio de agente no encontrada." });
            }

            return Ok(solicitud);
        }

        // GET: api/agente/solicitudes-cambio-agente
        [HttpGet("solicitudes-cambio-agente")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<SolicitudCambioAgenteResponseDTO>>> GetAllSolicitudesCambioAgente(string? estatus = null)
        {
            var solicitudesQuery = _context.SolicitudesCambioAgente
                .Include(s => s.AgenteVentaActual)
                .Include(s => s.AgenteVentaNuevo)
                .Include(s => s.Mayorista)
                .Include(s => s.Administrador)
                .AsQueryable(); // Hacer la consulta en forma de IQueryable

            // Filtrar por estatus si se proporciona
            if (!string.IsNullOrEmpty(estatus))
            {
                solicitudesQuery = solicitudesQuery.Where(s => s.Estatus == estatus);
            }

            var solicitudes = await solicitudesQuery.ToListAsync();

            if (solicitudes == null || !solicitudes.Any())
            {
                return Ok(new List<SolicitudCambioAgenteResponseDTO>());
            }

            var response = solicitudes.Select(s => new SolicitudCambioAgenteResponseDTO
            {
                Id = s.Id,
                Motivo = s.Motivo,
                Solicitante = s.Solicitante,
                Estatus = s.Estatus,
                FechaSolicitud = s.FechaSolicitud,
                FechaRespuesta = s.FechaRespuesta,

                IdMayorista = s.Mayorista.Id,
                NombreContacto = s.Mayorista.NombreContacto,
                CargoContacto = s.Mayorista.CargoContacto,
                TelefonoContacto = s.Mayorista.TelefonoContacto,
                EmailContacto = s.Mayorista.EmailContacto,

                IdAgenteVentaActual = s.AgenteVentaActual?.Id,
                AgenteVentaActualNombre = s.AgenteVentaActual?.FullName,
                IdAgenteVentaNuevo = s.AgenteVentaNuevo?.Id,
                AgenteVentaNuevoNombre = s.AgenteVentaNuevo?.FullName,

                IdAdministrador = s.Administrador?.Id,
                AdministradorNombre = s.Administrador?.FullName,
            });

            return Ok(response);
        }


        [HttpGet("solicitudes-cambio-agente/{mayoristaId}")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<SolicitudCambioAgenteResponseDTO>>> GetAllSolicitudesCambioAgente(int mayoristaId)
        {
            // Obtener las solicitudes del mayorista específico
            var solicitudes = await _context.SolicitudesCambioAgente
                .Include(s => s.AgenteVentaActual)
                .Include(s => s.AgenteVentaNuevo)
                .Include(s => s.Mayorista)
                .Include(s => s.Administrador)
                .Where(s => s.IdMayorista == mayoristaId) // Filtrar por el ID del mayorista
                .ToListAsync();

            if (solicitudes == null || !solicitudes.Any())
            {
                return NotFound(new { message = "No se encontraron solicitudes de cambio de agente para el mayorista especificado." });
            }

            var response = solicitudes.Select(s => new SolicitudCambioAgenteResponseDTO
            {
                Id = s.Id,
                Motivo = s.Motivo,
                Solicitante = s.Solicitante,
                Estatus = s.Estatus,
                FechaSolicitud = s.FechaSolicitud,
                FechaRespuesta = s.FechaRespuesta,
                MotivoRechazo = s.MotivoRechazo,

                IdMayorista = s.Mayorista.Id,
                NombreContacto = s.Mayorista.NombreContacto,
                CargoContacto = s.Mayorista.CargoContacto,
                TelefonoContacto = s.Mayorista.TelefonoContacto,
                EmailContacto = s.Mayorista.EmailContacto,

                IdAgenteVentaActual = s.AgenteVentaActual?.Id,
                AgenteVentaActualNombre = s.AgenteVentaActual?.FullName,
                IdAgenteVentaNuevo = s.AgenteVentaNuevo?.Id,
                AgenteVentaNuevoNombre = s.AgenteVentaNuevo?.FullName,

                IdAdministrador = s.Administrador?.Id,
                AdministradorNombre = s.Administrador?.FullName,
            });

            return Ok(response);
        }

        [HttpGet("ultima-solicitud/{mayoristaId}")]
        [AllowAnonymous]
        public async Task<ActionResult<SolicitudCambioAgenteResponseDTO>> GetUltimaSolicitudCambioAgente(int mayoristaId)
        {
            // Obtener la última solicitud del mayorista específico
            var ultimaSolicitud = await _context.SolicitudesCambioAgente
                .Include(s => s.AgenteVentaActual)
                .Include(s => s.AgenteVentaNuevo)
                .Include(s => s.Mayorista)
                .Include(s => s.Administrador)
                .Where(s => s.IdMayorista == mayoristaId) // Filtrar por el ID del mayorista
                .OrderByDescending(s => s.FechaSolicitud) // Ordenar por fecha de solicitud, descendente
                .FirstOrDefaultAsync(); // Obtener el último registro

            if (ultimaSolicitud == null)
            {
                return NotFound(new { message = "No se encontró ninguna solicitud de cambio de agente para el mayorista especificado." });
            }

            var response = new SolicitudCambioAgenteResponseDTO
            {
                Id = ultimaSolicitud.Id,
                Motivo = ultimaSolicitud.Motivo,
                Solicitante = ultimaSolicitud.Solicitante,
                Estatus = ultimaSolicitud.Estatus,
                FechaSolicitud = ultimaSolicitud.FechaSolicitud,
                FechaRespuesta = ultimaSolicitud.FechaRespuesta,
                MotivoRechazo = ultimaSolicitud.MotivoRechazo,

                IdMayorista = ultimaSolicitud.Mayorista.Id,
                NombreContacto = ultimaSolicitud.Mayorista.NombreContacto,
                CargoContacto = ultimaSolicitud.Mayorista.CargoContacto,
                TelefonoContacto = ultimaSolicitud.Mayorista.TelefonoContacto,
                EmailContacto = ultimaSolicitud.Mayorista.EmailContacto,

                IdAgenteVentaActual = ultimaSolicitud.AgenteVentaActual?.Id,
                AgenteVentaActualNombre = ultimaSolicitud.AgenteVentaActual?.FullName,
                IdAgenteVentaNuevo = ultimaSolicitud.AgenteVentaNuevo?.Id,
                AgenteVentaNuevoNombre = ultimaSolicitud.AgenteVentaNuevo?.FullName,

                IdAdministrador = ultimaSolicitud.Administrador?.Id,
                AdministradorNombre = ultimaSolicitud.Administrador?.FullName,
            };

            return Ok(response);
        }




    }
}