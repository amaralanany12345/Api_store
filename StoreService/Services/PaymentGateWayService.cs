using AutoMapper;
using Serilog;
using StoreService.DTO;
using StoreDomain.Enums;
using StoreService.Interfaces;
using StoreDomain.Models;
using Microsoft.Extensions.Logging;
using System.Transactions;

namespace StoreService.Services
{
    public class PaymentGateWayService : IPaymentGateWayService
    {
        private readonly IUnitOfWorkForWalletDb _walletDbUnitOfWork;
        private readonly IEmailService _emailService;
        private readonly IUnitOfWorkServiceForStoreDb _unitOfWork;
        private readonly ILogger<PaymentGateWayService> _logger;
        private readonly IMapper _mapper;
        private readonly IOrderService _orderService;
        private readonly IExternalLogService _externalLogService;
        public PaymentGateWayService(IEmailService emailService,IUnitOfWorkServiceForStoreDb unitOfWork,ILogger<PaymentGateWayService> logger,
            IMapper mapper,IOrderService orderService, IExternalLogService externalLogService, IUnitOfWorkForWalletDb walletDbUnitOfWork)
        {
            _emailService = emailService;
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
            _orderService = orderService;
            _externalLogService = externalLogService;
            _walletDbUnitOfWork = walletDbUnitOfWork;
        }

        public async Task<ReceiptDto> PayForOrder()
        {
            TransactionManager.ImplicitDistributedTransactions = true;
            using var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
            try
            {
                var order=await _orderService.GetOrder();
                var userWallet = await _walletDbUnitOfWork.Wallets.GetFirstOrDefault(a=>a.UserEmail==order.Customer.Email);
                if (userWallet == null)
                {
                    _logger.LogWarning("user wallet is not found");
                    throw new ArgumentException("user wallet is not found");
                }
                await _externalLogService.AddLog(SystemProvider.walletDbCall, order.Customer.Email,"call the wallet data base",
                    "success call the wallet database","ok 200","success");

                if (userWallet.Balance < order.TotalAmount)
                {

                    order.Status=OrderStatus.Cancelled.ToString();
                    order.TotalAmount = 0;
                    var orderItems = await _unitOfWork.OrderRepository.GetOrderItemsById(order.Id);
                    foreach(var orderItem in orderItems)
                    {
                        orderItem.Item.StockQuantity += orderItem.Quantity;
                        _unitOfWork.OrderItems.DeleteAsync(orderItem);
                        await _unitOfWork.SaveChangesAsync();
                    }
                    await _unitOfWork.SaveChangesAsync();
                    _logger.LogWarning("your balance is not enough");
                    throw new ArgumentException("your balance is not enough");
                }
                order.Status=OrderStatus.Approved.ToString();
                userWallet.Balance -= order.TotalAmount;
                //order.Customer.Balance =userWallet.Balance;
                await _externalLogService.AddLog(SystemProvider.paymentGateWay, order.Customer.Email, "payment success",
                    "approved the payment process", "ok 200", "success");
                var newReceipt=new Receipt
                {
                    orderId = order.Id,
                    Order=order,
                    CreatedAt = DateTime.Now,
                    TotalAmount=order.TotalAmount,
                };

                //var orderItemsInText = string.Join(" ", _unitOfWork.OrderItems.GetFirstOrDefault(a => a.OrderId == order.Id)
                //    .Select(a => $" item name is {a.Item.Name} and quantity needed is {a.Quantity} -"));

                var emailBody =$"your payment is approved and your order id is {order.Id},," +
                    $" total amount is {order.TotalAmount}, date is {newReceipt.CreatedAt}";
                await _externalLogService.AddLog(SystemProvider.emailService, order.Customer.Email, "send email",
                    "confirm that the email is send and the payment method is approved", "ok 200", "success");
                await _emailService.SendEmail(order.Customer.UserName,"success payment",emailBody);
                await _unitOfWork.Receipts.CreateAsync(newReceipt);
                await _unitOfWork.SaveChangesAsync();
                await _walletDbUnitOfWork.SaveChangeAsync();
                transaction.Complete();
                return _mapper.Map<ReceiptDto>(newReceipt);
            }
            catch(Exception ex)
            {
                _logger.LogWarning(ex,"payment is failed"); 
                transaction.Dispose();
                throw new Exception("payment is failed");
            }

        }
    }
}
