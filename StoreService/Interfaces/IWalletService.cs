using StoreDomain.Models;

namespace StoreService.Interfaces
{
    public interface IWalletService
    {
        Task<Wallet> CreateWalletToUser(string userEmail);
        Task<Wallet> GetWalletOfUser(string userEmail);
    }
}
