using Microsoft.EntityFrameworkCore;
using StoreDataBase.AppContexts;
using StoreService.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace StoreDataBase.Repositories
{
    public class GenericRepoServiceForWalletDb<T> : IGenericRepoService<T> where T : class
    {
        protected readonly WalletAppDbContext _context;

        public GenericRepoServiceForWalletDb(WalletAppDbContext context)
        {
            _context = context;
        }

        public async Task CreateAsync(T entity)
        {
            await _context.Set<T>().AddAsync(entity);
        }

        public async Task DeleteAsync(int entityId)
        {

            var entity = await GetAsync(entityId);
            _context.Remove(entity);
        }

        public async Task<List<T>> GetAllAsync()
        {
            return await _context.Set<T>().ToListAsync();
        }

        public async Task<T> GetAsync(int id)
        {
            return await _context.Set<T>().FindAsync(id);
        }

        public async Task<T> GetFirstOrDefault(Expression<Func<T, bool>> del)
        {
            return await _context.Set<T>().FirstOrDefaultAsync(del);
        }
    }
}
