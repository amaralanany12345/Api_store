namespace StoreWebApi.Interfaces
{
    public interface IEmail
    {
        void sendEmail(string toName, string toEmail, string subject, string content);
    }
}
