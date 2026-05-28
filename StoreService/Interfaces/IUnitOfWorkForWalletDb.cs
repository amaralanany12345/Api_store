using StoreDomain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoreService.Interfaces
{
    public interface IUnitOfWorkForWalletDb
    {
        IGenericRepoService<Wallet> Wallets { get; }
        Task<int> SaveChangeAsync();
    }
}
