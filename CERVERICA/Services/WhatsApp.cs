using RestSharp;

namespace CERVERICA.Services
{
    public class WhatsApp
    {
        private static readonly ILogger<WhatsApp> _logger = LoggerFactory.Create(builder =>
        {
            builder.AddConsole();
        }).CreateLogger<WhatsApp>();

        public static async Task SendWhatsAppMessage(string message, string number)
        {
            try
            {
                /*var url = "https://api.ultramsg.com/instance98771/messages/chat";*/
                var url = "https://api.ultramsg.com/instance98773/messages/chat";
                var client = new RestClient(url);

                var request = new RestRequest(url, Method.Post);
                request.AddHeader("content-type", "application/x-www-form-urlencoded");
                request.AddParameter("token", "zzjqyxcyp3frf6us");
                request.AddParameter("to", number);
                request.AddParameter("body", message);

                RestResponse response = await client.ExecuteAsync(request);
                var output = response.Content;
                Console.WriteLine(output);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending WhatsApp message");
            }
        }
    }
}
