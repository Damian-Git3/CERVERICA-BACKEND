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

        public NotificacionController(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Notificacion>>> GetNotificaciones()
        {
            try
            {
                var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                //List<Notificacion> notificaciones = await _context.Notificaciones.Where(n => n.IdUsuario == currentUserId).ToListAsync();
                //devolver solo las notificaciones con fecha y hora menor a la actual
                List<Notificacion> notificaciones = await _context.Notificaciones.Where(n => n.IdUsuario == currentUserId && n.Fecha <= DateTime.Now).ToListAsync();


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
    }
}

