using CERVERICA.Data;
using CERVERICA.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace CERVERICA.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class NotificacionController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        //private readonly FirebaseNotificationService _firebaseNotificationService;

        public NotificacionController(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
            //_firebaseNotificationService = firebaseNotificationService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Notificacion>>> GetNotificaciones()
        {
            try
            {
                var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                //List<Notificacion> notificaciones = await _context.Notificaciones.Where(n => n.IdUsuario == currentUserId).ToListAsync();
                //devolver solo las notificaciones con fecha y hora menor a la actual
                List<Notificacion> notificaciones = await _context.Notificaciones.Where(n => n.IdUsuario == currentUserId && n.Fecha <= DateTime.Now && n.Visto == false).ToListAsync();
                //ordenar las notificaciones por fecha descendente
                notificaciones = notificaciones.OrderByDescending(n => n.Fecha).ToList();

                return Ok(notificaciones);
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    isSuccessful = false,
                    message = "No se pudieron obtener las notificaciones del usuario"
                });
            }
        }

        //marcar una notificacion como vista
        [HttpPut("{id}")]
        public async Task<ActionResult<Notificacion>> PutNotificacion(int id)
        {
            try
            {
                var notificacion = await _context.Notificaciones.FindAsync(id);

                if (notificacion == null)
                {
                    return NotFound();
                }

                //revisar si la notificacion pertenece al usuario
                var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (notificacion.IdUsuario != currentUserId)
                {
                    return BadRequest(new
                    {
                        isSuccessful = false,
                        message = "La notificacion no pertenece al usuario"
                    });
                }

                if (notificacion.Visto == true)
                {
                    return BadRequest(new
                    {
                        isSuccessful = false,
                        message = "La notificacion ya fue marcada como vista"
                    });
                }


                notificacion.Visto = true;
                _context.Entry(notificacion).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                return Ok(new { message = "Notificación vista" });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    isSuccessful = false,
                    message = "No se pudo marcar la notificacion como vista"
                });
            }
        }

        //[HttpPost("send-notification")]
        //public async Task<IActionResult> SendNotification(string registrationToken, string title, string body)
        //{
        //    var result = await _firebaseNotificationService.SendNotificationAsync(registrationToken, title, body);
        //    return Ok(result);
        //}
    }
}

