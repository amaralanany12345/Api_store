using Microsoft.EntityFrameworkCore;
using StoreDataBase.AppContexts;
using StoreService.Interfaces;
using StoreService.ResultPattern;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace StoreDataBase.Repositories
{
    public class GenericRepoServiceForStoreDb<T> : IGenericRepoService<T> where T : class
    {
        protected readonly AppDbContext _context;

        public GenericRepoServiceForStoreDb(AppDbContext context)
        {
            _context = context;
        }

        public async Task CreateAsync(T entity)
        {
            await _context.Set<T>().AddAsync(entity);
        }

        public void DeleteAsync(T entity)
        {
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
