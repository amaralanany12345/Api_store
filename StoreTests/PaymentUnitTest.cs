using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using StoreService.DTO;
using StoreService.Interfaces;
using StoreDomain.Models;
using StoreService.Services;
using StoreDataBase.AppContexts;
using StoreDomain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StoreService.ResponseModel;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Http;

namespace StoreTests
{
    public class PaymentServiceTest
    {
        private readonly IPaymentGateWayService _paymentService;
        private readonly Mock<ILogger<PaymentGateWayService>> _loggerMock;
        private readonly AppDbContext _appDbContext;
        private readonly WalletAppDbContext _walletAppDbContext;
        private readonly Mock<IGenericRepoService<Receipt>> _genericRepoMock;
        private readonly Mock<IUnitOfWorkServiceForStoreDb> _unitOfWorkMock;
        private readonly Mock<IUnitOfWorkForWalletDb> _unitOfWorkForWalletDbMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IEmailService> _emailServiceMock;
        private readonly Mock<IOrderService> _orderServiceMock;
        private readonly Mock<IExternalLogService> _externalLogServiceMock;
        public PaymentServiceTest()
        {
            _externalLogServiceMock = new Mock<IExternalLogService>();
            _emailServiceMock = new Mock<IEmailService>();
            _orderServiceMock = new Mock<IOrderService>();
            var appDbContextOptions = new DbContextOptionsBuilder<AppDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
            var WalletAppDbContextOptions = new DbContextOptionsBuilder<WalletAppDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
            _appDbContext = new AppDbContext(appDbContextOptions);
            _walletAppDbContext = new WalletAppDbContext(WalletAppDbContextOptions);
            _genericRepoMock = new Mock<IGenericRepoService<Receipt>>();
            _mapperMock = new Mock<IMapper>();
            _loggerMock = new Mock<ILogger<PaymentGateWayService>>();
            _unitOfWorkMock = new Mock<IUnitOfWorkServiceForStoreDb>();
            _unitOfWorkForWalletDbMock = new Mock<IUnitOfWorkForWalletDb>();
            _paymentService = new PaymentGateWayService(_emailServiceMock.Object, _unitOfWorkMock.Object, _loggerMock.Object,
                _mapperMock.Object, _orderServiceMock.Object, _externalLogServiceMock.Object, _unitOfWorkForWalletDbMock.Object);
        }
        [Fact]
        public async Task PayForOrder_ReturnReceipt()
        {
            var newCustomer = new User
            {
                Id=1,
                UserName="saad",
                Email="saad@gmail.com",
                PasswordHash=BCrypt.Net.BCrypt.HashPassword("saad123"),
                //Balance=3000,
                Role=UserRole.Customer.ToString(),
                CreatedAt=DateTime.Now,
            };
            var newOrder = new Order
            {
                 Id=1, 
                CreatedAt=DateTime.Now,
                Status=OrderStatus.InProgress.ToString(),
                TotalAmount=200,
                Customer=newCustomer,
                CustomerId=newCustomer.Id,
            };
            var newReceipt = new Receipt
            {
                Id = 1,
                CreatedAt = DateTime.Now,
                TotalAmount=newOrder.TotalAmount,
                orderId=newOrder.Id,
                Order=newOrder,
            };
            var newWallet = new Wallet
            {
                Id=1,
                UserEmail="saad@gmail.com",
                Balance=3000,
                Currency="USD $"
            };
            await _walletAppDbContext.Wallets.AddAsync(newWallet);
            await _walletAppDbContext.SaveChangesAsync();
            await _appDbContext.Users.AddAsync(newCustomer);
            await _appDbContext.Orders.AddAsync(newOrder);
            await _appDbContext.Receipts.AddAsync(newReceipt);
            await _appDbContext.SaveChangesAsync();
            var newReceiptDto = new ReceiptDto
            {
                TotalAmount=newOrder.TotalAmount,
                CreateAt=DateTime.Now,
            };
            _orderServiceMock.Setup(x => x.GetOrder()).ReturnsAsync(ResultResponse<Order>.Pass(newOrder,StatusCodes.Status200OK));
            _unitOfWorkForWalletDbMock.Setup(a => a.Wallets.GetFirstOrDefault(It.IsAny<Expression<Func<Wallet,bool>>>())).ReturnsAsync(newWallet);
            _unitOfWorkMock.Setup(a => a.Receipts.CreateAsync(It.IsAny<Receipt>())).Returns(Task.CompletedTask);
            _unitOfWorkMock.Setup(a => a.SaveChangesAsync()).ReturnsAsync(1);
            _unitOfWorkForWalletDbMock.Setup(a => a.SaveChangeAsync()).ReturnsAsync(1);
            _mapperMock.Setup(a => a.Map<ReceiptDto>(It.IsAny<Receipt>())).Returns(newReceiptDto);
            var result = await _paymentService.PayForOrder();
            Assert.Equal(newReceiptDto.TotalAmount, result.Result.TotalAmount);
            Assert.NotNull(result);

        }
    }
}
