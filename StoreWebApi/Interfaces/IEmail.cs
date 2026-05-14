namespace StoreWebApi.Interfaces
{
    public interface IEmail
    {
        Task sendEmail(string toName,  string subject, string content);
    }
}
