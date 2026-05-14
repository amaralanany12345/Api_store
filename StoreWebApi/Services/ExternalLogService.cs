using StoreWebApi.Enums;
using StoreWebApi.Interfaces;
using StoreWebApi.Models;
using StoreWebApi.zAppContexts;

namespace StoreWebApi.Services
{
    public class ExternalLogService : IExternalLog
    {
        private readonly AppDbContext _appDbContext;

        public ExternalLogService(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public async Task<ExternalLog> addLog(SystemProvider provider,string userEmail, string operation, string requestPayload, string responsePayLoad, string status)
        {
            var newExternalLog = new ExternalLog
            {
                Provider=provider.ToString(),
                UserEmail=userEmail,
                Operation=operation,
                RequestPayload=requestPayload,
                ResponsePayload=responsePayLoad,
                Status=status,
                CreatedAt=DateTime.Now,
            };
            await _appDbContext.AddAsync(newExternalLog);
            await _appDbContext.SaveChangesAsync();
            return newExternalLog;
        }
    }
}
