using CERVERICA.Services;
using Microsoft.AspNetCore.Mvc;

namespace CERVERICA.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MailController : Controller
    {
        [HttpPost]
        public async Task<ActionResult<object>> SendMail(string subject, string body, string to)
        {
            try
            {

                await MailService.SendMail(subject, body, to);

                return Ok(new { message = "Correo enviado correctamente" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Ocurrió un error al enviar el correo.", details = ex.Message });
            }
        }
    }
}
