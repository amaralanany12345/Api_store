namespace StoreService.Interfaces
{
    public interface IEmailService
    {
        Task SendEmail(string toName,  string subject, string content);
    }
}
