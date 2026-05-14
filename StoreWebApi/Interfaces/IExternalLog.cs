using StoreWebApi.Enums;
using StoreWebApi.Models;

namespace StoreWebApi.Interfaces
{
    public interface IExternalLog
    {
        Task<ExternalLog> addLog(SystemProvider provider,string userEmail, string operation,string requestPayload,string responsePayLoad ,string status);
    }
}
