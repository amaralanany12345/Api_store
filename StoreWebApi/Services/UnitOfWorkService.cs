using StoreWebApi.Interfaces;
using StoreWebApi.zAppContexts;

namespace StoreWebApi.Services
{
    public class UnitOfWorkService : IUnitOfWork
    {
        private readonly AppDbContext _appDbContext;

        public UnitOfWorkService(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public async Task<int> saveChangesAsync()
        {
            return await _appDbContext.SaveChangesAsync();
        }
    }
}
