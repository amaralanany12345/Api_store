using StoreDomain.Enums;
using StoreService.Interfaces;
using StoreDomain.Models;

namespace StoreService.Services
{
    public class ExternalLogService : IExternalLogService
    {
        private readonly IUnitOfWorkServiceForStoreDb _unitOfWorkService;

        public ExternalLogService(IUnitOfWorkServiceForStoreDb unitOfWorkService)
        {
            _unitOfWorkService = unitOfWorkService;
        }

        public async Task<ExternalLog> AddLog(SystemProvider provider,string userEmail, string operation, string requestPayload, string responsePayLoad, string status)
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
            await _unitOfWorkService.ExternalLogs.CreateAsync(newExternalLog);
            await _unitOfWorkService.SaveChangesAsync();
            return newExternalLog;
        }
    }
}
