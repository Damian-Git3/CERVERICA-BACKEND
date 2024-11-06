using CERVERICA.Services;
using Microsoft.AspNetCore.Mvc;

namespace CERVERICA.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WhatsAppController : Controller
    {
        [HttpPost]
        public async Task<ActionResult<object>> SendWhatsAppMessage(string message, string number)
        {
            try
            {

                await WhatsAppService.SendWhatsAppMessage(message, number);
                return Ok(new { message = "Mensaje enviado correctamente" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Ocurrió un error al enviar el mensaje de WhatsApp.", details = ex.Message });
            }
        }
    }
}
