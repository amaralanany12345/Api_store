using StoreDomain.Enums;
using StoreDomain.Models;


namespace StoreService.Interfaces
{
    public interface IExternalLogService
    {
        Task<ExternalLog> AddLog(SystemProvider provider,string userEmail, string operation,string requestPayload,string responsePayLoad ,string status);
    }
}
