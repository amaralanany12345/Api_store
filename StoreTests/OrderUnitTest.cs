using AutoMapper;
using Castle.Core.Resource;
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
    public class OrderServiceTest
    {
        private readonly AppDbContext _context;
        private readonly Mock<IMapper>_mapperMock;
        private readonly Mock<IGenericRepo<Order>> _genericRepoMock;
        private readonly Mock<IUnitOfWork>_unitOfWorkMock;
        private readonly Mock<ILogger<OrderService>> _loggerMock;
        private readonly Mock<IUser>_userServiceMock;
        private readonly OrderService _orderService;
        public OrderServiceTest()
        {
            var appDbContextOptions = new DbContextOptionsBuilder<AppDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
            _context = new AppDbContext(appDbContextOptions);
            _mapperMock= new Mock<IMapper>();
            _genericRepoMock=new Mock<IGenericRepo<Order>>();
            _unitOfWorkMock=new Mock<IUnitOfWork>();
            _loggerMock=new Mock<ILogger<OrderService>>();
            _userServiceMock=new Mock<IUser>();
            _orderService = new OrderService(_context,_mapperMock.Object,_genericRepoMock.Object,_unitOfWorkMock.Object,
                _loggerMock.Object,_userServiceMock.Object);

        }
        [Fact]
        public async Task CreateOrder_ReturnOrderDto()
        {
            var customer = new User
            {
                Id = 1,
                UserName="ammar",
                Email="ammar@gmail.com",
                PasswordHash=BCrypt.Net.BCrypt.HashPassword("ammar123"),
                CreatedAt=DateTime.Now,
                Balance=3000,
                Role=UserRole.Customer.ToString(),
            };
            var newOrder = new Order
            {
                Id=1,
                CustomerId=customer.Id,
                Customer=customer,
                CreatedAt= DateTime.Now,
                Status=OrderStatus.InProgress.ToString(),
                TotalAmount=0
            };
            await _context.Users.AddAsync(customer);
            await _context.Orders.AddAsync(newOrder);
            await _context.SaveChangesAsync();
            var newOrderDto = new OrderDto
            {
                CreatedAt = DateTime.Now,
                TotalAmount=0,
                Status=OrderStatus.InProgress.ToString(),
                
            };
            _userServiceMock.Setup(a => a.getCurrentUser()).ReturnsAsync(customer);
            _mapperMock.Setup(a => a.Map<OrderDto>(It.IsAny<Order>())).Returns(newOrderDto);
            var result = await _orderService.createOrder();
            Assert.NotNull(result);
            Assert.Equal(newOrderDto.TotalAmount, result.TotalAmount);
        }
        [Fact]
        public async Task GetAllOrders_ReturnAllOrders()
        {
            var listOfOrders = new List<Order>
            {
                new Order{Id = 1,CustomerId =1,CreatedAt = DateTime.Now,Status = OrderStatus.InProgress.ToString(),TotalAmount = 100 },
                new Order{Id = 2,CustomerId =2,CreatedAt = DateTime.Now,Status = OrderStatus.InProgress.ToString(),TotalAmount = 30 },
                new Order{Id = 3,CustomerId =3,CreatedAt = DateTime.Now,Status = OrderStatus.InProgress.ToString(),TotalAmount = 40 },
            };
            await _context.Orders.AddRangeAsync(listOfOrders);
            await _context.SaveChangesAsync();
            var listOfOrdersDto = new List<OrderDto>
            {
                new OrderDto{CreatedAt = DateTime.Now,TotalAmount = 100,Status = OrderStatus.InProgress.ToString(),},
                new OrderDto{CreatedAt = DateTime.Now,TotalAmount = 30,Status = OrderStatus.InProgress.ToString(),},
                new OrderDto{CreatedAt = DateTime.Now,TotalAmount = 40,Status = OrderStatus.InProgress.ToString(),},

            };
            _mapperMock.Setup(a => a.Map<List<OrderDto>>(It.IsAny<List<Order>>())).Returns(listOfOrdersDto);
            var result = await _orderService.getAllOrders();
            Assert.NotNull(result);
            Assert.Equal(3, result.Count);
            Assert.Equal(listOfOrders[2].TotalAmount, result[2].TotalAmount);
        }
        [Fact]
        public async Task AddOrderITemToOrder_ByItemIdAndQuantity_ReturnOrderItem()
        {
            var customer = new User
            {
                Id = 1,
                UserName = "ammar",
                Email = "ammar@gmail.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("ammar123"),
                CreatedAt = DateTime.Now,
                Balance = 3000,
                Role = UserRole.Customer.ToString(),
            };
            var newOrder = new Order
            {
                Id = 1,
                CustomerId = 1,
                CreatedAt = DateTime.Now,
                Status = OrderStatus.InProgress.ToString(),
                TotalAmount = 0
            };
            var newITem = new Item
            {
                Id=1,
                Name="math",
                Price=100,
                StockQuantity=30,
                CategoryId=1,
            };
            var newOrderItem = new OrderItem
            {
                Order = newOrder,
                OrderId = newOrder.Id,
                Item = newITem,
                ItemId = newITem.Id,
                Quantity = 2
            };
            await _context.Users.AddAsync(customer);
            await _context.Orders.AddAsync(newOrder);
            await _context.Items.AddAsync(newITem);
            await _context.SaveChangesAsync();
            _userServiceMock.Setup(a => a.getCurrentUser()).ReturnsAsync(customer);
            var result = await _orderService.AddOrderItemToOrder(newITem.Id,2);
            Assert.NotNull(result);
            Assert.Equal(newOrderItem.Quantity,result.Quantity);
        }
        [Fact]
        public async Task DeleteOrderItemFromOrder_ByItemId_RemoveORderItem()
        {
            var customer = new User
            {
                Id = 1,
                UserName = "ammar",
                Email = "ammar@gmail.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("ammar123"),
                CreatedAt = DateTime.Now,
                Balance = 3000,
                Role = UserRole.Customer.ToString(),
            };
            var newOrder = new Order
            {
                Id = 1,
                CustomerId = 1,
                CreatedAt = DateTime.Now,
                Status = OrderStatus.InProgress.ToString(),
                TotalAmount = 0
            };
            var newITem = new Item
            {
                Id = 1,
                Name = "math",
                Price = 100,
                StockQuantity = 30,
                CategoryId = 1,
            };
            var newOrderItem = new OrderItem
            {
                Order = newOrder,
                OrderId = newOrder.Id,
                Item = newITem,
                ItemId = newITem.Id,
                Quantity = 2
            };
            await _context.Users.AddAsync(customer);
            await _context.Orders.AddAsync(newOrder);
            await _context.Items.AddAsync(newITem);
            await _context.OrderItem.AddAsync(newOrderItem);
            await _context.SaveChangesAsync();
            _userServiceMock.Setup(a => a.getCurrentUser()).ReturnsAsync(customer);
            await _orderService.deleteOrderItemFromOrder(newITem.Id);
            var deletedOrderItem=await _context.OrderItem.FirstOrDefaultAsync(a=>a.OrderId == newOrder.Id && a.ItemId==newITem.Id);
            Assert.Null(deletedOrderItem);
        }
        [Fact]
        public async Task GetOrderItems_ReturnOrderItems()
        {
            var customer = new User
            {
                Id = 1,
                UserName = "ammar",
                Email = "ammar@gmail.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("ammar123"),
                CreatedAt = DateTime.Now,
                Balance = 3000,
                Role = UserRole.Customer.ToString(),
            };
            var newOrder = new Order
            {
                Id = 1,
                CustomerId = customer.Id,
                CreatedAt = DateTime.Now,
                Status = OrderStatus.InProgress.ToString(),
                TotalAmount = 0
            };
            var newITem = new Item
            {
                Id = 1,
                Name = "math",
                Price = 100,
                StockQuantity = 30,
                CategoryId = 1,
            };
            var newITem2 = new Item
            {
                Id = 2,
                Name = "english",
                Price = 100,
                StockQuantity = 30,
                CategoryId = 1,
            };
            var ListOfOrderItem = new List<OrderItem>
            {
                new OrderItem{OrderId = newOrder.Id,ItemId =1,Quantity = 2},
                new OrderItem{OrderId = newOrder.Id,ItemId =2,Quantity = 2},
            };
            await _context.Users.AddAsync(customer);
            await _context.Orders.AddAsync(newOrder);
            await _context.Items.AddAsync(newITem);
            await _context.Items.AddAsync(newITem2);
            await _context.OrderItem.AddRangeAsync(ListOfOrderItem);
            await _context.SaveChangesAsync();
            _userServiceMock.Setup(a => a.getCurrentUser()).ReturnsAsync(customer);
            var result = await _orderService.getOrderItems();
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
        }
        [Fact]
        public async Task GetOrderItems_ByOrderId_ReturnOrderItems()
        {
            var customer = new User
            {
                Id = 1,
                UserName = "ammar",
                Email = "ammar@gmail.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("ammar123"),
                CreatedAt = DateTime.Now,
                Balance = 3000,
                Role = UserRole.Customer.ToString(),
            };
            var newOrder = new Order
            {
                Id = 1,
                CustomerId = customer.Id,
                CreatedAt = DateTime.Now,
                Status = OrderStatus.InProgress.ToString(),
                TotalAmount = 0
            };
            var mathItem = new Item
            {
                Id = 1,
                Name = "math",
                Price = 100,
                StockQuantity = 30,
                CategoryId = 1,
            };
            var englishItem = new Item
            {
                Id = 2,
                Name = "english",
                Price = 100,
                StockQuantity = 30,
                CategoryId = 1,
            };
            var ListOfOrderItem = new List<OrderItem>
            {
                new OrderItem{OrderId = newOrder.Id,ItemId =mathItem.Id,Quantity = 2},
                new OrderItem{OrderId = newOrder.Id,ItemId =englishItem.Id,Quantity = 4},
            };
            await _context.Users.AddAsync(customer);
            await _context.Orders.AddAsync(newOrder);
            await _context.Items.AddAsync(mathItem);
            await _context.Items.AddAsync(englishItem);
            await _context.OrderItem.AddRangeAsync(ListOfOrderItem);
            await _context.SaveChangesAsync();
            _userServiceMock.Setup(a => a.getCurrentUser()).ReturnsAsync(customer);
            var listOrderITemDto = new List<OrderItemDto>
            {
                new OrderItemDto{Price=mathItem.Price,Quantity=2,ItemName=mathItem.Name},
                new OrderItemDto{Price=englishItem.Price,Quantity=4,ItemName=englishItem.Name},
            };
            _mapperMock.Setup(a=>a.Map<List<OrderItemDto>>(It.IsAny<List<OrderItem>>())).Returns(listOrderITemDto);
            var result = await _orderService.getOrderItemsById(newOrder.Id);
            Assert.NotNull(result);
            Assert.Equal(ListOfOrderItem[0].Quantity, result[0].Quantity);
        }
        [Fact]
        public async Task CancelOrder_OrderCancelled()
        {
            var customer = new User
            {
                Id = 1,
                UserName = "ammar",
                Email = "ammar@gmail.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("ammar123"),
                CreatedAt = DateTime.Now,
                Balance = 3000,
                Role = UserRole.Customer.ToString(),
            };
            var newOrder = new Order
            {
                Id = 1,
                CustomerId = customer.Id,
                CreatedAt = DateTime.Now,
                Status = OrderStatus.InProgress.ToString(),
                TotalAmount = 100
            };
            var mathItem = new Item
            {
                Id = 1,
                Name = "math",
                Price = 100,
                StockQuantity = 30,
                CategoryId = 1,
            };
            var englishItem = new Item
            {
                Id = 2,
                Name = "english",
                Price = 100,
                StockQuantity = 30,
                CategoryId = 1,
            };
            var ListOfOrderItem = new List<OrderItem>
            {
                new OrderItem{OrderId = newOrder.Id,ItemId =mathItem.Id,Quantity = 2},
                new OrderItem{OrderId = newOrder.Id,ItemId =englishItem.Id,Quantity = 4},
            };
            await _context.Users.AddAsync(customer);
            await _context.Orders.AddAsync(newOrder);
            await _context.Items.AddAsync(mathItem);
            await _context.Items.AddAsync(englishItem);
            await _context.OrderItem.AddRangeAsync(ListOfOrderItem);
            await _context.SaveChangesAsync();
            _userServiceMock.Setup(a => a.getCurrentUser()).ReturnsAsync(customer);
            await _orderService.CancelOrder();
            Assert.Equal(0,newOrder.TotalAmount);
            Assert.Equal(OrderStatus.Cancelled.ToString(),newOrder.Status);
        }
    }
}
