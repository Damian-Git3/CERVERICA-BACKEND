using MailKit.Net.Smtp;
using MimeKit;

namespace CERVERICA.Services
{
    public class MailService
    {
        private static readonly ILogger<MailService> _logger = LoggerFactory.Create(builder =>
        {
            builder.AddConsole();
        }).CreateLogger<MailService>();

        public static async Task SendMail(string subject, string body, string to)
        {
            try
            {
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress("Damian Gamboa", "damiangl0310@gmail.com"));
                message.To.Add(new MailboxAddress("", to));
                message.Subject = subject;

                message.Body = new TextPart("plain")
                {
                    Text = body
                };

                /* EJEMPLO */
                //message.Body = new TextPart("plain")
                //{
                //    Text = @"Hey Chandler,

                //    I just wanted to let you know that Monica and I were going to go play some paintball, you in?

                //    -- Joey"
                //};

                using var client = new SmtpClient();
                await client.ConnectAsync("smtp.gmail.com", 587, MailKit.Security.SecureSocketOptions.StartTls);

                // Note: only needed if the SMTP server requires authentication
                await client.AuthenticateAsync("damiangl0310@gmail.com", "rpfp ulxr iaxo dabg");

                await client.SendAsync(message);
                await client.DisconnectAsync(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending email");
            }
        }
    }


}