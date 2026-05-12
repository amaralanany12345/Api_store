using MailKit.Net.Smtp;
using MimeKit;
using Serilog;
using StoreWebApi.Interfaces;
using StoreWebApi.Models;

namespace StoreWebApi.Services
{
    public class EmailService : IEmail
    {
        private readonly IConfiguration _configuration;
        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void sendEmail(string toName, string toEmail, string subject, string content)
        {
            var emailSender = _configuration.GetSection("SmtpSettings").Get<EmailSenderModel>();
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("store", "storereply@domain.com"));
            message.To.Add(new MailboxAddress(toName, toEmail));
            message.Subject = subject;

            message.Body = new TextPart("plain")
            {
                Text = content
            };

            using (var client = new SmtpClient())
            {
                client.Connect(emailSender.SmtpServer,emailSender.Port, false);

                // Note: only needed if the SMTP server requires authentication
                client.Authenticate(emailSender.Login,emailSender.Password);

                var result = client.Send(message);
                Console.WriteLine($"message is send {result}");
                client.Disconnect(true);
            }
        }
    }
}
