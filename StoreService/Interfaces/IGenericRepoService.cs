using StoreService.ResultPattern;
using System.Linq.Expressions;

namespace StoreService.Interfaces
{
    public interface IGenericRepoService<T> where T : class
    {
        Task CreateAsync(T entity);
        Task<T> GetAsync(int id);
        Task<List<T>> GetAllAsync();
        void DeleteAsync(T entity);
        Task<T> GetFirstOrDefault(Expression<Func<T,bool>> del);
    }
}
