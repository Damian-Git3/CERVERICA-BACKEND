using CERVERICA.Data;
using CERVERICA.Dtos;
using CERVERICA.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace CERVERICA.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SolicitudesMayoristasController : ControllerBase
    {
        private readonly ApplicationDbContext _db;

        public SolicitudesMayoristasController(UserManager<ApplicationUser> userManager, ApplicationDbContext db)
        {
            _db = db;
        }

        [HttpGet("obtener-solicitudes-mayoristas")]
        public async Task<ActionResult<SolicitudMayorista>> obtenerSolicitudesMayoristas()

        {
            var idUsuario = HttpContext.Items["idUsuario"] as string;

            var solicitudesMayoristas = await _db.SolicitudesMayorista
            .Where(s => s.IdAgente == idUsuario)
            .Include(s => s.Mayorista)
            .ToListAsync();

            return Ok(solicitudesMayoristas);
        }
    }
}
