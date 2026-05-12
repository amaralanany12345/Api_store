using AutoMapper;
using StoreWebApi.Interfaces;
using StoreWebApi.zAppContexts;

namespace StoreWebApi.Services
{
    public class GenericRepoService<T> : IGenericRepo<T> where T : class
    {
        private readonly AppDbContext _appDbContext;
        public GenericRepoService(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public async Task CreateAsync(T item)
        {
             await _appDbContext.Set<T>().AddAsync(item);
        }
    }
}
