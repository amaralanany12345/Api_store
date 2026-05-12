namespace StoreWebApi.Interfaces
{
    public interface IUnitOfWork
    {
        Task<int> saveChangesAsync();
    }
}
