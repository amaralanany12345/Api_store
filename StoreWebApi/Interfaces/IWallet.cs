using StoreWebApi.Models;

namespace StoreWebApi.Interfaces
{
    public interface IWallet
    {
        Task<Wallet> createWalletToUser(string userEmail);
        Task<Wallet> getWalletOfUser(string userEmail);
    }
}
