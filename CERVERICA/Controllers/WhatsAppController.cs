using CERVERICA.Services;
using Microsoft.AspNetCore.Mvc;

namespace CERVERICA.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class WhatsAppController(ILogger<WhatsAppController> logger) : ControllerBase
    {
        [HttpPost]
        public async Task<ActionResult<object>> SendWhatsAppMessage(string message, string number)
        {


            try
            {
                var whatsappService = new WhatsAppService();
                await whatsappService.SendWhatsAppMessage(message, number);
                return Ok(new { message = "Mensaje enviado correctamente" });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error sending WhatsApp message");
                return StatusCode(500, new { message = "Ocurrió un error al enviar el mensaje de WhatsApp.", details = ex.Message });
            }
        }
    }
}
