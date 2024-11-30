using CERVERICA.Data;
using CERVERICA.Dtos;
using CERVERICA.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CERVERICA.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class SolicitudAsistenciaController : ControllerBase
    {

        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public SolicitudAsistenciaController(UserManager<ApplicationUser> userManager,
                                             RoleManager<IdentityRole> roleManager,
                                             ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
            _roleManager = roleManager;
        }

        // Crear una nueva categoría de asistencia (solo Admin y Agente)
        [HttpPost("categoria")]
        [Authorize(Roles = "Admin,Agente")]
        public async Task<ActionResult> CrearCategoria(CrearCategoriaDTO categoriaDTO)
        {
            var categoria = new CategoriaAsistencia
            {
                Nombre = categoriaDTO.Nombre,
                Estatus = true
            };
            _context.CategoriasAsistencia.Add(categoria);
            await _context.SaveChangesAsync();
            return Ok(categoria);
        }

        // Ver todas las categorías de asistencia activas
        [HttpGet("categorias")]
        public async Task<IActionResult> ObtenerCategorias()
        {
            var categorias = _context.CategoriasAsistencia
                .Select(c => new CategoriaAsistenciaDTO
                {
                    Id = c.Id,
                    Nombre = c.Nombre,
                    Estatus = c.Estatus
                }).Where(c => c.Estatus == true).ToList();


            return Ok(categorias);
        }

        // Ver todas las categorías de asistencia (solo Admin y Agente)
        [HttpGet("categorias/all")]
        [Authorize(Roles = "Admin,Agente")]
        public async Task<IActionResult> ObtenerTodasCategorias()
        {
            var categorias = _context.CategoriasAsistencia
                .Select(c => new CategoriaAsistenciaDTO
                {
                    Id = c.Id,
                    Nombre = c.Nombre,
                    Estatus = c.Estatus
                }).ToList();

            return Ok(categorias);
        }

        // Marcar categoría como inactiva (solo Admin y Agente)
        [HttpPut("categoria/inactivar")]
        [Authorize(Roles = "Admin,Agente")]
        public async Task<IActionResult> MarcarCategoriaInactiva(SeleccionarCategoriaDTO categoriaDTO)
        {
            var categoria = await _context.CategoriasAsistencia.FindAsync(categoriaDTO.Id);
            if (categoria == null)
                return NotFound();

            categoria.Estatus = false;
            await _context.SaveChangesAsync();
            return Ok();
        }

        // Marcar categoría como activa (solo Admin y Agente)
        [HttpPut("categoria/activar")]
        [Authorize(Roles = "Admin,Agente")]
        public async Task<IActionResult> MarcarCategoriaActiva(SeleccionarCategoriaDTO categoriaDTO)
        {
            var categoria = await _context.CategoriasAsistencia.FindAsync(categoriaDTO.Id);
            if (categoria == null)
                return NotFound();

            categoria.Estatus = true;
            await _context.SaveChangesAsync();
            return Ok();
        }

        // Crear una nueva solicitud de asistencia (Cliente y Mayorista)
        [HttpPost("solicitud")]
        [Authorize(Roles = "Cliente,Mayorista")]
        public async Task<ActionResult<SolicitudAsistenciaDTO>> CrearSolicitud(SolicitudAsistenciaDTO solicitudDTO)
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            var agenteAsignadoId = AsignarAgente();

            //averiguar si el usuario tiene el rol de mayorista
            var mayoristaRoleId = _context.Roles.Where(r => r.Name == "Mayorista").Select(r => r.Id).FirstOrDefault();
            var esMayorista = _context.UserRoles.Any(ur => ur.UserId == user.Id && ur.RoleId == mayoristaRoleId);

            if (!esMayorista && solicitudDTO.Mayoreo == true)
                return BadRequest(new { message = "No puedes solicitar asistencia de mayoreo si no eres mayorista", error = true });

            var solicitud = new SolicitudAsistencia
            {
                IdCliente = user.Id,
                IdAgenteVenta = agenteAsignadoId,
                IdCategoriaAsistencia = solicitudDTO.IdCategoriaAsistencia,
                Mayoreo = solicitudDTO.Mayoreo,
                Descripcion = solicitudDTO.Descripcion,
                FechaSolicitud = DateTime.Now,
                Estatus = 1 // Activa
            };
            _context.SolicitudesAsistencia.Add(solicitud);


            await _context.SaveChangesAsync();
            return Ok(solicitud);
        }

        // Ver historial de asistencias de un cliente (Cliente y Mayorista)
        [HttpGet("agente/historial")]
        [Authorize(Roles = "Admin,Agente")]
        public async Task<IActionResult> ObtenerSolicitudesAsignadas()
        {

            var user = await _userManager.GetUserAsync(HttpContext.User);

            var historial = (from s in _context.SolicitudesAsistencia
                             where s.IdAgenteVenta == user.Id && s.FechaCierre == null
                             join c in _context.CategoriasAsistencia on s.IdCategoriaAsistencia equals c.Id
                             join u in _context.Users on s.IdCliente equals u.Id
                             select new
                             {
                                 s.Id,
                                 s.IdCliente,
                                 s.IdAgenteVenta,
                                 s.IdCategoriaAsistencia,
                                 s.Mayoreo,
                                 s.Descripcion,
                                 s.FechaSolicitud,
                                 s.FechaCierre,
                                 s.Estatus,
                                 s.Valoracion,
                                 s.MensajeValoracion,
                                 s.Tipo,
                                 nombreCategoria = c.Nombre,
                                 nombreCliente = u.FullName,
                                 numeroCliente = u.PhoneNumber,
                                 emailCliente = u.Email
                             }).OrderByDescending(s => s.FechaSolicitud)
                .ToList();

            return Ok(historial);
        }

        [HttpGet("agente/historialCerrado")]
        [Authorize(Roles = "Admin,Agente")]
        public async Task<IActionResult> ObtenerSolicitudesAsignadasCerradas()
        {

            var user = await _userManager.GetUserAsync(HttpContext.User);

            var historial = (from s in _context.SolicitudesAsistencia
                             where s.IdAgenteVenta == user.Id && s.FechaCierre != null
                             join c in _context.CategoriasAsistencia on s.IdCategoriaAsistencia equals c.Id
                             join u in _context.Users on s.IdCliente equals u.Id
                             select new
                             {
                                 s.Id,
                                 s.IdCliente,
                                 s.IdAgenteVenta,
                                 s.IdCategoriaAsistencia,
                                 s.Mayoreo,
                                 s.Descripcion,
                                 s.FechaSolicitud,
                                 s.FechaCierre,
                                 s.Estatus,
                                 s.Valoracion,
                                 s.MensajeValoracion,
                                 s.Tipo,
                                 nombreCategoria = c.Nombre,
                                 nombreCliente = u.FullName,
                                 numeroCliente = u.PhoneNumber,
                                 emailCliente = u.Email
                             }).OrderByDescending(s => s.FechaSolicitud)
                .ToList();

            return Ok(historial);
        }

        // Ver historial de asistencias de un cliente (Cliente y Mayorista)
        [HttpGet("agente/historial/cliente")]
        [Authorize(Roles = "Admin,Agente")]
        public async Task<ActionResult> ObtenerHistorial(string id)
        {
            
            var historial = _context.SolicitudesAsistencia
                .Where(s => s.IdCliente == id && s.FechaCierre == null)
                .ToList();

            return Ok(historial);
        }

        // Ver historial de asistencias de un cliente (Cliente y Mayorista)
        [HttpGet("cliente/historial")]
        [Authorize(Roles = "Cliente,Mayorista")]
        public async Task<IActionResult> ObtenerHistorialCliente()
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);

            var historial = (from s in _context.SolicitudesAsistencia
                             where s.IdCliente == user.Id && s.FechaCierre == null
                             join c in _context.CategoriasAsistencia on s.IdCategoriaAsistencia equals c.Id
                             join u in _context.Users on s.IdAgenteVenta equals u.Id
                             select new
                             {
                                 s.Id,
                                 s.IdCliente,
                                 s.IdAgenteVenta,
                                 s.IdCategoriaAsistencia,
                                 s.Mayoreo,
                                 s.Descripcion,
                                 s.FechaSolicitud,
                                 s.FechaCierre,
                                 s.Estatus,
                                 s.Valoracion,
                                 s.MensajeValoracion,
                                 s.Tipo,
                                 nombreCategoria = c.Nombre,
                                 nombreAgente = u.FullName,
                                 numeroAgente = u.PhoneNumber,
                                 emailAgente = u.Email
                             }).OrderByDescending(s => s.FechaSolicitud)
                .ToList();

            return Ok(historial);
        }

        //ruta para ver los detalles de una sola solicitud de asistencia con los seguimientos
        [HttpGet("solicitud/{id}")]
        [Authorize(Roles = "Cliente,Mayorista,Agente,Admin")]
        public async Task<ActionResult> ObtenerSolicitud(int id)
        {
            var solicitud = _context.SolicitudesAsistencia.Find(id);
            if (solicitud == null) return NotFound();

            var seguimientos = _context.SeguimientosSolicitudesAsistencia
                .Where(s => s.IdSolicitudAsistencia == id)
                .ToList();

            //incluir los datos de la categoria de asistencia y los datos del agente de venta
            var categoria = _context.CategoriasAsistencia.Find(solicitud.IdCategoriaAsistencia);
            var agente = _context.Users.Find(solicitud.IdAgenteVenta);
            var cliente = _context.Users.Find(solicitud.IdCliente);

            solicitud.SeguimientosSolicitudAsistencia = seguimientos;

            //crear un nuevo objeto con los datos de numero, nombre y telefono del cliente
            var clienteData = new
            {
                cliente.FullName,
                cliente.UserName,
                cliente.PhoneNumber,
                cliente.Email,
                cliente.FechaRegistro,
                cliente.Activo
            };
            return Ok(new { solicitud, clienteData });
        }   

        [HttpGet("cliente/historialCerrado/agente")]
        [Authorize(Roles = "Admin,Agente")]
        public async Task<ActionResult> ObtenerHistorialCerrado(string id)
        {

            var historial = _context.SolicitudesAsistencia
                .Where(s => s.IdCliente == id && s.FechaCierre != null)
                .ToList();

            return Ok(historial);
        }

        // Ver historial de asistencias de un cliente (Cliente y Mayorista)
        [HttpGet("cliente/historialCerrado")]
        [Authorize(Roles = "Cliente,Mayorista")]
        public async Task<IActionResult> ObtenerHistorialClienteCerrado()
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);

            var historial = (from s in _context.SolicitudesAsistencia
                             where s.IdCliente == user.Id && s.FechaCierre != null
                             join c in _context.CategoriasAsistencia on s.IdCategoriaAsistencia equals c.Id
                             join u in _context.Users on s.IdAgenteVenta equals u.Id
                             select new
                             {
                                 s.Id,
                                 s.IdCliente,
                                 s.IdAgenteVenta,
                                 s.IdCategoriaAsistencia,
                                 s.Mayoreo,
                                 s.Descripcion,
                                 s.FechaSolicitud,
                                 s.FechaCierre,
                                 s.Estatus,
                                 s.Valoracion,
                                 s.MensajeValoracion,
                                 s.Tipo,
                                 nombreCategoria = c.Nombre,
                                 nombreAgente = u.FullName,
                                 numeroAgente = u.PhoneNumber,
                                 emailAgente = u.Email
                             }).OrderByDescending(s => s.FechaSolicitud)
                .ToList();

            return Ok(historial);
        }


        // Marcar asistencia como cerrada (Admin y Agente)
        [HttpPut("solicitud/cerrar")]
        [Authorize(Roles = "Admin,Agente")]
        public async Task<ActionResult> CerrarAsistencia(CerrarSolicitudAsistenciaDTO solicitudDTO)
        {
            var asistencia = await _context.SolicitudesAsistencia.FindAsync(solicitudDTO.IdSolicitudAsistencia);
            if (asistencia == null) return NotFound();

            //verificar que la asistencia pertenezca al usuario
            var user = await _userManager.GetUserAsync(HttpContext.User);

            if (asistencia.IdAgenteVenta != user.Id) return Unauthorized();

            asistencia.FechaCierre = DateTime.Now;
            asistencia.Estatus = 3; // Cerrada
            //asistencia.Descripcion = solicitudDTO.Descripcion;

            await _context.SaveChangesAsync();
            return Ok();
        }

        // Marcar asistencia como cerrada (Cliente y Mayorista)
        [HttpPut("solicitud/eliminar")]
        [Authorize(Roles = "Cliente,Mayorista")]
        public async Task<ActionResult> EliminarAsistencia(CerrarSolicitudAsistenciaDTO solicitudDTO)
        {
            var asistencia = await _context.SolicitudesAsistencia.FindAsync(solicitudDTO.IdSolicitudAsistencia);
            if (asistencia == null) return NotFound();

            //verificar que la asistencia pertenezca al usuario
            var user = await _userManager.GetUserAsync(HttpContext.User);

            if (asistencia.IdCliente != user.Id) return Unauthorized();

            asistencia.FechaCierre = DateTime.Now;
            asistencia.Estatus = 3; // Cerrada
            asistencia.Descripcion = solicitudDTO.Descripcion;

            await _context.SaveChangesAsync();
            return Ok();
        }

        // Valorar una asistencia (Cliente y Mayorista)
        [HttpPut("solicitud/valorar")]
        [Authorize(Roles = "Cliente,Mayorista")]
        public async Task<ActionResult> ValorarAsistencia(SolicitudValoracionDTO valoracionDTO)
        {
            var asistencia = await _context.SolicitudesAsistencia.FindAsync(valoracionDTO.IdSolicitudAsistencia);
            var user = await _userManager.GetUserAsync(HttpContext.User);
            if (asistencia == null || asistencia.IdCliente != user.Id)
                return Unauthorized();

            asistencia.Valoracion = valoracionDTO.Valoracion;
            asistencia.MensajeValoracion = valoracionDTO.Mensaje;
            await _context.SaveChangesAsync();
            return Ok();
        }

        // Generar seguimiento de una asistencia (solo Agente)
        [HttpPost("solicitud/seguimiento")]
        [Authorize(Roles = "Agente")]
        public async Task<ActionResult> GenerarSeguimiento(SeguimientoSolicitudAsistenciaDTO seguimientoDTO)
        {
            var asistencia = await _context.SolicitudesAsistencia.FindAsync(seguimientoDTO.IdSolicitudAsistencia);
            if (asistencia == null) return NotFound();

            //verificar que la asistencia no esté cerrada
            if (asistencia.Estatus == 3) return BadRequest(new { message = "La asistencia ya está cerrada", error = true });

            var seguimiento = new SeguimientoSolicitudAsistencia
            {
                IdSolicitudAsistencia = seguimientoDTO.IdSolicitudAsistencia,
                Descripcion = seguimientoDTO.Descripcion,
                FechaSeguimiento = DateTime.Now,
                Mensaje = seguimientoDTO.Mensaje
            };

            _context.SeguimientosSolicitudesAsistencia.Add(seguimiento);
            //dar estatus 2 a la solicitud
            asistencia.Estatus = 2; // En proceso
            await _context.SaveChangesAsync();
            return Ok(seguimiento);
        }

        [HttpGet("solicitud/seguimientos")]
        [Authorize(Roles = "Agente,Admin,Cliente,Mayorista")]
        public async Task<ActionResult> ObtenerSeguimientos(int id)
        {
            var asistencia = await _context.SolicitudesAsistencia.FindAsync(id);
            if (asistencia == null) return NotFound();

            var seguimientos = _context.SeguimientosSolicitudesAsistencia
                .Where(s => s.IdSolicitudAsistencia == id)
                .ToList();

            return Ok(seguimientos);
        }


        //Metodo para reasignar agente de ventas a una solicitud
        [HttpPut("solicitud/reasignar")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> ReasignarAgente(ReasignarAgenteDTO reasignarDTO)
        {
            var solicitud = await _context.SolicitudesAsistencia.FindAsync(reasignarDTO.IdSolicitudAsistencia);
            if (solicitud == null) return NotFound();

            var user = await _userManager.GetUserAsync(HttpContext.User);

            if (solicitud.IdCliente != user.Id) return Unauthorized();

            solicitud.IdAgenteVenta = reasignarDTO.IdAgenteVenta;
            await _context.SaveChangesAsync();
            return Ok();
        }

        
        //solicitar cambio de agente
        [HttpPut("solicitud/cambiarAgente")]
        [Authorize(Roles = "Cliente, Mayorista")]
        public async Task<ActionResult> CambiarAgente(SolicitudValoracionDTO solicitudDTO)
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);

            //obtener a la id del agente actual de la solicitud
            var solicitud = await _context.SolicitudesAsistencia.FindAsync(solicitudDTO.IdSolicitudAsistencia);

            if (solicitud.IdCliente != user.Id) return Unauthorized();

            if (solicitud == null) return NotFound();

            var idAgenteNuevo = CambiarAgente(solicitud.IdAgenteVenta);

            var agenteNuevo = _context.Users.Find(idAgenteNuevo);
            var agenteAnterior = _context.Users.Find(solicitud.IdAgenteVenta);

            var seguimiento = new SeguimientoSolicitudAsistencia
            {
                IdSolicitudAsistencia = solicitudDTO.IdSolicitudAsistencia,
                Descripcion = "Solicitud de cambio de agente",
                FechaSeguimiento = DateTime.Now,
                Mensaje = "Nombre y id Agente nuevo: " + agenteNuevo.FullName + " - " + agenteNuevo.Id + ". Nombre y id Agente anterior: " + agenteAnterior.FullName + " - " + agenteAnterior.Id + "." + solicitudDTO.Mensaje
            };

            _context.SeguimientosSolicitudesAsistencia.Add(seguimiento);

            //cerrar la solicitud
            solicitud.FechaCierre = DateTime.Now;
            solicitud.Estatus = 3; // Cerrada
            solicitud.MensajeValoracion = solicitudDTO.Mensaje;
            solicitud.Valoracion = solicitudDTO.Valoracion;

            //clonar la solicitud con el nuevo agente
            var nuevaSolicitud = new SolicitudAsistencia
            {
                IdCliente = solicitud.IdCliente,
                IdAgenteVenta = idAgenteNuevo,
                IdCategoriaAsistencia = solicitud.IdCategoriaAsistencia,
                Mayoreo = solicitud.Mayoreo,
                Descripcion = solicitud.Descripcion,
                FechaSolicitud = DateTime.Now,
                Estatus = 1, // Activa
                Tipo = 0,
                IdVenta = solicitud.IdVenta,
                IdSolicitudMayorista = solicitud.IdSolicitudMayorista
            };

            _context.SolicitudesAsistencia.Add(nuevaSolicitud);
            await _context.SaveChangesAsync(); // Guardar para obtener el Id generado

            // Generar un seguimiento para la nueva solicitud utilizando el Id generado
            var seguimientoNuevo = new SeguimientoSolicitudAsistencia
            {
                IdSolicitudAsistencia = nuevaSolicitud.Id, // Utilizar el Id generado
                Descripcion = "Solicitud derivada de un cambio de agente",
                FechaSeguimiento = DateTime.Now,
                Mensaje = "Debido a: " + solicitudDTO.Mensaje
            };

            _context.SeguimientosSolicitudesAsistencia.Add(seguimientoNuevo);
            await _context.SaveChangesAsync(); // Guardar el seguimiento
            return Ok(solicitud);
        }


        // Método para asignar agente de ventas con menos clientes
        private string AsignarAgente()
        {
            var agenteVentasRoleId = _context.Roles.Where(r => r.Name == "Agente").Select(r => r.Id).FirstOrDefault();
            var agentesVentas = _context.UserRoles
                .Where(ur => ur.RoleId == agenteVentasRoleId)
                .Select(ur => ur.UserId)
                .ToList();

            var agenteConMenosClientes = _context.ClientesMayoristas
                .GroupBy(cm => cm.IdAgenteVenta)
                .Where(g => agentesVentas.Contains(g.Key))
                .Select(g => new { IdAgenteVenta = g.Key, Count = g.Count() })
                .OrderBy(g => g.Count)
                .FirstOrDefault();

            return agenteConMenosClientes == null ? agentesVentas.FirstOrDefault() : agenteConMenosClientes.IdAgenteVenta;
        }


        private string CambiarAgente(string IdAgenteAnterior)
        {
            var agenteVentasRoleId = _context.Roles.Where(r => r.Name == "Agente").Select(r => r.Id).FirstOrDefault();

            var agentesVentas = _context.UserRoles
                .Where(ur => ur.RoleId == agenteVentasRoleId && ur.UserId != IdAgenteAnterior)
                .Select(ur => ur.UserId)
                .ToList();

            var agenteConMenosClientes = _context.ClientesMayoristas
                .GroupBy(cm => cm.IdAgenteVenta)
                .Where(g => agentesVentas.Contains(g.Key))
                .Select(g => new { IdAgenteVenta = g.Key, Count = g.Count() })
                .OrderBy(g => g.Count)
                .FirstOrDefault();

            return agenteConMenosClientes == null ? agentesVentas.FirstOrDefault() : agenteConMenosClientes.IdAgenteVenta;
        }


    }
}
