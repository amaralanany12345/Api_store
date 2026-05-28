using StoreDataBase.AppContexts;
using StoreDomain.Models;
using StoreService.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoreDataBase.Repositories
{
    public class UnitOfWorkOfWalletDb : IUnitOfWorkForWalletDb
    {
        private readonly WalletAppDbContext _walletDbContext;
        public IGenericRepoService<Wallet> Wallets { get; } 

        public UnitOfWorkOfWalletDb(WalletAppDbContext walletDbContext)
        {
            _walletDbContext = walletDbContext;
            Wallets=new GenericRepoServiceForWalletDb<Wallet>(walletDbContext);
        }


        public async Task<int> SaveChangeAsync()
        {
            return await _walletDbContext.SaveChangesAsync();
        }
    }
}
