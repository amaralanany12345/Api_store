using MailKit.Net.Smtp;
using MimeKit;
using Serilog;

namespace StoreWebApi.Models
{
    public class EmailSenderModel
    {
        public string SmtpServer { get; set; }
        public int Port { get; set; }
        public string Login { get; set; }
        public string Password { get;set; }
    }
}
