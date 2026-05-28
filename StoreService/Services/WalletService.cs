using StoreService.Interfaces;
using StoreDomain.Models;

namespace StoreService.Services
{
    public class WalletService : IWalletService
    {
        private readonly IUnitOfWorkServiceForStoreDb _unitOfWorkService;
        private readonly IUnitOfWorkForWalletDb _walletDbUnitOfWork;
        public WalletService(IUnitOfWorkServiceForStoreDb unitOfWorkService, IUnitOfWorkForWalletDb walletDbUnitOfWork)
        {
            _unitOfWorkService = unitOfWorkService;
            _walletDbUnitOfWork = walletDbUnitOfWork;
        }

        public async Task<Wallet> CreateWalletToUser(string userEmail)
        {
            var newWallet=new Wallet
            {
                Balance = 5000,
                UserEmail = userEmail,
                Currency="USD $"
                
            };
            await _walletDbUnitOfWork.Wallets.CreateAsync(newWallet);
            await _unitOfWorkService.SaveChangesAsync();
            return newWallet;
        }

        public async Task<Wallet> GetWalletOfUser(string userEmail)
        {
            var userWallet = await _walletDbUnitOfWork.Wallets.GetFirstOrDefault(a => a.UserEmail == userEmail);
            //_walletDbContext.Wallets.Where(a=>a.UserEmail==userEmail).FirstOrDefaultAsync();
            if(userWallet==null)
            {
                throw new ArgumentException("user wallet is not found");
            }
            return userWallet;
        }
    }
}
