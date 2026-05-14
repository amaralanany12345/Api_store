using Microsoft.EntityFrameworkCore;
using StoreWebApi.Interfaces;
using StoreWebApi.Models;
using StoreWebApi.zAppContexts;

namespace StoreWebApi.Services
{
    public class WalletService : IWallet
    {
        private readonly WalletAppDbContext _walletDbContext;

        public WalletService(WalletAppDbContext walletDbContext)
        {
            _walletDbContext = walletDbContext;
        }

        public async Task<Wallet> createWalletToUser(string userEmail)
        {
            var newWallet=new Wallet
            {
                Balance = 5000,
                UserEmail = userEmail,
                Currency="USD $"
                
            };
            await _walletDbContext.AddAsync(newWallet);
            await _walletDbContext.SaveChangesAsync();
            return newWallet;
        }

        public async Task<Wallet> getWalletOfUser(string userEmail)
        {
            var userWallet=await _walletDbContext.Wallets.Where(a=>a.UserEmail==userEmail).FirstOrDefaultAsync();
            if(userWallet==null)
            {
                throw new ArgumentException("user wallet is not found");
            }
            return userWallet;
        }
    }
}
