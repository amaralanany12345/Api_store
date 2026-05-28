using StoreService.Services;
using StoreDataBase.AppContexts;
using StoreService.Interfaces;
using Microsoft.EntityFrameworkCore;
using StoreDomain.Models;
using StoreService.RepositoriesInterfaces;

namespace StoreDataBase.Repositories
{
    public class UnitOfWorkServiceOfStoreDb : IUnitOfWorkServiceForStoreDb
    {
        private readonly AppDbContext _appDbContext;
        public IGenericRepoService<Order> Orders { get; }
        public IGenericRepoService<OrderItem> OrderItems { get; }
        public IGenericRepoService<Item> Items { get; }
        public IGenericRepoService<User> Users { get; }
        public IGenericRepoService<Category> Categories { get; } 
        public IGenericRepoService<Receipt> Receipts { get; }
        public IGenericRepoService<RefreshToken> RefreshTokens { get; }
        public IOrderRepository OrderRepository { get; }
        public IUserRepository UserRepository { get; }
        public IITemRepository ITemRepository { get; }
        public IGenericRepoService<ExternalLog> ExternalLogs { get; }

        public UnitOfWorkServiceOfStoreDb(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
            Orders = new GenericRepoServiceForStoreDb<Order>(appDbContext);
            OrderItems = new GenericRepoServiceForStoreDb<OrderItem>(appDbContext);
            Items = new GenericRepoServiceForStoreDb<Item>(appDbContext);
            Categories=new GenericRepoServiceForStoreDb<Category>(appDbContext);
            Users = new GenericRepoServiceForStoreDb<User>(appDbContext);
            Receipts=new GenericRepoServiceForStoreDb<Receipt>(appDbContext);
            RefreshTokens=new GenericRepoServiceForStoreDb<RefreshToken>(appDbContext);
            ExternalLogs=new GenericRepoServiceForStoreDb<ExternalLog>(appDbContext);
            OrderRepository=new OrderRepository(appDbContext);
            UserRepository=new UserRepository(appDbContext);
            ITemRepository=new ItemRepository(appDbContext);
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _appDbContext.SaveChangesAsync();
        }
    }
}
