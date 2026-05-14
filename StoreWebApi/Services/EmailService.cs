using MailKit.Net.Smtp;
using MimeKit;
using Org.BouncyCastle.Crypto.Macs;
using Serilog;
using StoreWebApi.Interfaces;
using StoreWebApi.Models;
using System.Net;

namespace StoreWebApi.Services
{
    public class EmailService : IEmail
    {
        private readonly IConfiguration _configuration;
        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task sendEmail(string toName, string subject, string content)
        {
            var emailSender = _configuration.GetSection("SmtpSettings").Get<EmailSenderModel>();
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("store", "aalanany09@gmail.com"));
            message.To.Add(new MailboxAddress(toName, "aalanany09@gmail.com"));
            message.Subject = subject;

            message.Body = new TextPart("plain")
            {
                Text = content
            };

            using (var client = new SmtpClient())
            {
                await client.ConnectAsync(emailSender.SmtpServer,emailSender.Port, MailKit.Security.SecureSocketOptions.StartTls);
                await client.AuthenticateAsync(emailSender.Login,emailSender.Password);
                await client.SendAsync(message);
                await client.DisconnectAsync(true);
            }
        }
    }
}
