using StoreDomain.Models;
using StoreService.RepositoriesInterfaces;

namespace StoreService.Interfaces
{
    public interface IUnitOfWorkServiceForStoreDb
    {
        Task<int> SaveChangesAsync();
        IGenericRepoService<Order> Orders { get; }
        IGenericRepoService<OrderItem> OrderItems { get; }
        IGenericRepoService<Item> Items { get; }
        IGenericRepoService<User> Users { get; }
        IGenericRepoService<Category> Categories { get; }
        IGenericRepoService<Receipt> Receipts { get; }
        IGenericRepoService<RefreshToken> RefreshTokens { get; }
        IGenericRepoService<ExternalLog> ExternalLogs { get; }
        IOrderRepository OrderRepository { get; }
        IUserRepository UserRepository { get; }
        IITemRepository ITemRepository { get; }
    }
}
