using AutoMapper;
using Serilog;
using StoreService.DTO;
using StoreDomain.Enums;
using StoreService.Interfaces;
using StoreDomain.Models;
using Microsoft.Extensions.Logging;
using System.Transactions;
using Microsoft.AspNetCore.Http;
using StoreService.ResponseModel;

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

        public async Task<ResultResponse<ReceiptDto>> PayForOrder()
        {
            TransactionManager.ImplicitDistributedTransactions = true;
            using var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
            try
            {
                var order=await _orderService.GetOrder();
                var userWallet = await _walletDbUnitOfWork.Wallets.GetFirstOrDefault(a=>a.UserEmail==order.Result.Customer.Email);
                if (userWallet == null)
                {
                    _logger.LogWarning("user wallet is not found");
                    return ResultResponse<ReceiptDto>.Fail("user wallet is not found",ErrorTypes.NotFound,StatusCodes.Status404NotFound);
                    //throw new ArgumentException("user wallet is not found");
                }
                await _externalLogService.AddLog(SystemProvider.walletDbCall, order.Result.Customer.Email,"call the wallet data base",
                    "success call the wallet database","ok 200","success");

                if (userWallet.Balance < order.Result.TotalAmount)
                {

                    order.Result.Status=OrderStatus.Cancelled.ToString();
                    order.Result.TotalAmount = 0;
                    var orderItems = await _unitOfWork.OrderRepository.GetOrderItemsById(order.Result.Id);
                    await _unitOfWork.OrderRepository.DeleteOrderItems(order.Result.Id);
                    await _unitOfWork.SaveChangesAsync();
                    _logger.LogWarning("your balance is not enough");
                    return ResultResponse<ReceiptDto>.Fail("your balance is not enough", ErrorTypes.BadRequest,StatusCodes.Status400BadRequest);
                }
                order.Result.Status=OrderStatus.Approved.ToString();
                userWallet.Balance -= order.Result.TotalAmount;
                //order.Customer.Balance =userWallet.Balance;
                await _externalLogService.AddLog(SystemProvider.paymentGateWay, order.Result.Customer.Email, "payment success",
                    "approved the payment process", "ok 200", "success");
                var newReceipt=new Receipt
                {
                    orderId = order.Result.Id,
                    Order=order.Result,
                    CreatedAt = DateTime.Now,
                    TotalAmount=order.Result.TotalAmount,
                };

                //var orderItemsInText = string.Join(" ", _unitOfWork.OrderItems.GetFirstOrDefault(a => a.OrderId == order.Id)
                //    .Select(a => $" item name is {a.Item.Name} and quantity needed is {a.Quantity} -"));

                var emailBody =$"your payment is approved and your order id is {order.Result.Id},," +
                    $" total amount is {order.Result.TotalAmount}, date is {newReceipt.CreatedAt}";
                await _externalLogService.AddLog(SystemProvider.emailService, order.Result.Customer.Email, "send email",
                    "confirm that the email is send and the payment method is approved", "ok 200", "success");
                await _emailService.SendEmail(order.Result.Customer.UserName,"success payment",emailBody);
                await _unitOfWork.Receipts.CreateAsync(newReceipt);
                await _unitOfWork.SaveChangesAsync();
                await _walletDbUnitOfWork.SaveChangeAsync();
                transaction.Complete();
                return ResultResponse<ReceiptDto>.Pass(_mapper.Map<ReceiptDto>(newReceipt),StatusCodes.Status201Created);
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
