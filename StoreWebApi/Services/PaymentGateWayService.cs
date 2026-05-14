using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Serilog;
using StoreWebApi.DTO;
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
        private readonly IMapper _mapper;
        private readonly IOrder _orderService;
        private readonly IExternalLog _externalLogService;
        public PaymentGateWayService(AppDbContext appDbContext, WalletAppDbContext walletAppDbContext, IEmail emailService,
            IGenericRepo<Receipt> genericRepo, IUnitOfWork unitOfWork, ILogger<PaymentGateWayService> logger, IMapper mapper, IOrder orderService, IExternalLog externalLogService)
        {
            _appDbContext = appDbContext;
            _walletAppDbContext = walletAppDbContext;
            _emailService = emailService;
            _genericRepo = genericRepo;
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
            _orderService = orderService;
            _externalLogService = externalLogService;
        }

        public async Task<ReceiptDto> payForOrder()
        {
            var order=await _orderService.getOrder();
            var userWallet = await _walletAppDbContext.Wallets.Where(a => a.UserEmail == order.Customer.Email).FirstOrDefaultAsync();
            if (userWallet == null)
            {
                _logger.LogWarning("user wallet is not found");
                throw new ArgumentException("user wallet is not found");
            }
            await _externalLogService.addLog(SystemProvider.walletDbCall, order.Customer.Email,"call the wallet data base",
                "success call the wallet database","ok 200","success");

            if (userWallet.Balance < order.TotalAmount)
            {

                order.Status=OrderStatus.Cancelled.ToString();
                order.TotalAmount = 0;
                var orderItems = await _appDbContext.OrderItem.Where(a => a.OrderId == order.Id).Include(a => a.Item).ToListAsync();
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
            await _externalLogService.addLog(SystemProvider.paymentGateWay, order.Customer.Email, "payment success",
                "approved the payment process", "ok 200", "success");
            await _appDbContext.SaveChangesAsync();
            await _walletAppDbContext.SaveChangesAsync();
            var newReceipt=new Receipt
            {
                orderId = order.Id,
                Order=order,
                CreatedAt = DateTime.Now,
                TotalAmount=order.TotalAmount,
            };

            var orderItemsInText = string.Join(" ", _appDbContext.OrderItem.Where(a=>a.OrderId==order.Id)
                .Select(a =>$" item name is {a.Item.Name} and quantity needed is {a.Quantity} -"));

            var emailBody =$"your payment is approved and your order id is {order.Id},{orderItemsInText}," +
                $" total amount is {order.TotalAmount}, date is {newReceipt.CreatedAt}";
            await _externalLogService.addLog(SystemProvider.emailService, order.Customer.Email, "send email",
                "confirm that the email is send and the payment method is approved", "ok 200", "success");
            await _emailService.sendEmail(order.Customer.UserName,"success payment",emailBody);
            await _genericRepo.CreateAsync(newReceipt);
            await _unitOfWork.saveChangesAsync();
            return _mapper.Map<ReceiptDto>(newReceipt);

        }
    }
}
