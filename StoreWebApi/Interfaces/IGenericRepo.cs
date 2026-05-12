namespace StoreWebApi.Interfaces
{
    public interface IGenericRepo<T> where T : class
    {
        Task CreateAsync(T item);
    }
}
