using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using StoreWebApi.DTO;
using StoreWebApi.Enums;
using StoreWebApi.Interfaces;
using StoreWebApi.Models;
using StoreWebApi.Services;
using StoreWebApi.zAppContexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoreTests
{
    public class PaymentServiceTest
    {
        private readonly IPaymentGateWay _paymentService;
        private readonly Mock<ILogger<PaymentGateWayService>> _loggerMock;
        private readonly AppDbContext _appDbContext;
        private readonly WalletAppDbContext _walletAppDbContext;
        private readonly Mock<IGenericRepo<Receipt>> _genericRepoMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IEmail> _emailServiceMock;
        private readonly Mock<IOrder> _orderServiceMock;
        private readonly Mock<IExternalLog> _externalLogServiceMock;
        public PaymentServiceTest()
        {
            _externalLogServiceMock = new Mock<IExternalLog>();
            _emailServiceMock= new Mock<IEmail>();
            _orderServiceMock = new Mock<IOrder>();
            var appDbContextOptions = new DbContextOptionsBuilder<AppDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
            var WalletAppDbContextOptions = new DbContextOptionsBuilder<WalletAppDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
            _appDbContext=new AppDbContext(appDbContextOptions);
            _walletAppDbContext = new WalletAppDbContext(WalletAppDbContextOptions);
            _genericRepoMock = new Mock<IGenericRepo<Receipt>>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _mapperMock = new Mock<IMapper>();
            _loggerMock = new Mock<ILogger<PaymentGateWayService>>();
            _paymentService = new PaymentGateWayService(_appDbContext, _walletAppDbContext,_emailServiceMock.Object,_genericRepoMock.Object,
                _unitOfWorkMock.Object,_loggerMock.Object,_mapperMock.Object,_orderServiceMock.Object,_externalLogServiceMock.Object);
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
                Balance=3000,
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
            _orderServiceMock.Setup(x => x.getOrder()).ReturnsAsync(newOrder);
            _mapperMock.Setup(a => a.Map<ReceiptDto>(It.IsAny<Receipt>())).Returns(newReceiptDto);
            var result = await _paymentService.payForOrder();
            Assert.Equal(newReceiptDto.TotalAmount, result.TotalAmount);
            Assert.NotNull(result);

        }
    }
}
