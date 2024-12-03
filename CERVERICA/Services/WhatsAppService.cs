using DotNetEnv;
using RestSharp;


namespace CERVERICA.Services
{
    public class WhatsAppService
    {
        private readonly ILogger<WhatsAppService> _logger;

        public WhatsAppService()
        {
        }

        public WhatsAppService(ILogger<WhatsAppService> logger)
        {
            _logger = logger;

        }

        public async Task SendWhatsAppMessage(string message, string number)
        {
            try
            {
                var url = Env.GetString("INSTANCIA_WSP");
                var client = new RestClient(url);
                var request = new RestRequest(url, Method.Post);

                request.AddHeader("content-type", "application/x-www-form-urlencoded");
                request.AddParameter("token", Env.GetString("TOKEN_WSP"));
                request.AddParameter("to", "+52" + number);
                request.AddParameter("body", message);

                RestResponse response = await client.ExecuteAsync(request);
                var output = response.Content;
                _logger.LogInformation("WhatsApp message sent: {Output}", output);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending WhatsApp message");
            }
        }
    }
}
