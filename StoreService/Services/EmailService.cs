using MailKit.Net.Smtp;
using MimeKit;
using Org.BouncyCastle.Crypto.Macs;
using Serilog;
using StoreService.Interfaces;
using StoreDomain.Models;
using System.Net;
using Microsoft.Extensions.Configuration;

namespace StoreService.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendEmail(string toName, string subject, string content)
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
