using Microsoft.EntityFrameworkCore;
using Serilog;
using StoreWebApi.Enums;
using StoreWebApi.Interfaces;
using StoreWebApi.Models;
using StoreWebApi.zAppContexts;

namespace StoreWebApi.Services
{
    public class PaymentGateWayService : IPaymentGateWay
    {
        private readonly AppDbContext _appDbContext;
        private readonly WalletAppDbContext _walletAppDbContext;
        private readonly IEmail _emailService;
        private readonly IGenericRepo<Receipt> _genericRepo;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<PaymentGateWayService> _logger;
        public PaymentGateWayService(AppDbContext appDbContext, WalletAppDbContext walletAppDbContext, IEmail emailService, IGenericRepo<Receipt> genericRepo, IUnitOfWork unitOfWork, ILogger<PaymentGateWayService> logger)
        {
            _appDbContext = appDbContext;
            _walletAppDbContext = walletAppDbContext;
            _emailService = emailService;
            _genericRepo = genericRepo;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<Receipt> payForOrder(int orderId)
        {
            var order=await _appDbContext.Orders.Where(a=>a.Id==orderId).Include(a=>a.Customer).Include(A=>A.OrderItems).FirstOrDefaultAsync();
            if (order==null)
            {
                _logger.LogWarning("order is not found ");
                throw new ArgumentException("order is not found");
            }
            var userWallet = await _walletAppDbContext.Wallets.Where(a => a.UserEmail == order.Customer.Email).FirstOrDefaultAsync();
            if (userWallet == null)
            {
                _logger.LogWarning("user wallet is not found");
                throw new ArgumentException("user wallet is not found");
            }

            if (userWallet.Balance < order.TotalAmount)
            {

                order.Status=OrderStatus.Cancelled.ToString();
                order.TotalAmount = 0;
                var orderItems = await _appDbContext.OrderItem.Where(a => a.OrderId == orderId).Include(a => a.Item).ToListAsync();
                foreach(var orderItem in orderItems)
                {
                    orderItem.Item.StockQuantity += orderItem.Quantity;
                    _appDbContext.OrderItem.Remove(orderItem);
                    await _appDbContext.SaveChangesAsync();
                }
                await _appDbContext.SaveChangesAsync();
                _logger.LogWarning("your balance is not enough");
                throw new ArgumentException("your balance is not enough");
            }
            order.Status=OrderStatus.Approved.ToString();
            userWallet.Balance -= order.TotalAmount;
            order.Customer.Balance =userWallet.Balance;
            await _appDbContext.SaveChangesAsync();
            await _walletAppDbContext.SaveChangesAsync();
            var newReceipt=new Receipt
            {
                orderId = orderId,
                Order=order,
                CreatedAt = DateTime.Now,
                TotalAmount=order.TotalAmount,
            };
            _emailService.sendEmail(order.Customer.UserName,order.Customer.Email
                ,"success payment",$"your payment is approved and your balance is reduced by {order.TotalAmount} and your current balance is {userWallet.Balance}");
            await _genericRepo.CreateAsync(newReceipt);
            await _unitOfWork.saveChangesAsync();
            return newReceipt;

        }
    }
}
