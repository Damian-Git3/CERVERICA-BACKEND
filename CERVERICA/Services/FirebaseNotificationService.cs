using Google.Apis.Auth.OAuth2;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

namespace CERVERICA.Services
{
    public class FirebaseNotificationService
    {
        private readonly HttpClient _httpClient;
        private readonly string _firebaseUrl = "https://fcm.googleapis.com/v1/projects/1040299181492/messages:send";
        private readonly GoogleCredential _googleCredential;

        public FirebaseNotificationService()
        {
            // Cargar el archivo de credenciales de servicio
            //_googleCredential = GoogleCredential.FromFile("./serviceAccountKey.json")
            //    .CreateScoped("https://www.googleapis.com/auth/firebase.messaging");

            //_httpClient = new HttpClient();
        }

        public async Task<string> SendNotificationAsync(string registrationToken, string title, string body)
        {
            // Autenticación para obtener el token de acceso
            var accessToken = await _googleCredential.UnderlyingCredential.GetAccessTokenForRequestAsync();

            // Crear el payload de la notificación
            var message = new
            {
                message = new
                {
                    token = registrationToken,
                    notification = new
                    {
                        title = title,
                        body = body
                    }
                }
            };

            // Serializar el mensaje
            var jsonMessage = JsonConvert.SerializeObject(message);

            // Configurar el request
            var request = new HttpRequestMessage(HttpMethod.Post, _firebaseUrl);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            request.Content = new StringContent(jsonMessage, Encoding.UTF8, "application/json");

            // Enviar la notificación
            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsStringAsync();
        }
    }
}
