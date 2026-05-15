using AutoMapper;
using Castle.Core.Logging;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using StoreWebApi.Interfaces;
using StoreWebApi.Models;
using StoreWebApi.Services;
using StoreWebApi.zAppContexts;
using System;
using System.Net.Http;
using System.Net;
using StoreWebApi.DTO;

namespace StoreTests
{
    public class ItemServiceTests
    {
        private readonly Mock<IGenericRepo<Item>> _genericRepoMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ILogger<ItemService>> _loggerMock;
        private readonly AppDbContext _context;
        private readonly ItemService _itemService;

        public ItemServiceTests()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
            _context = new AppDbContext(options);
            _genericRepoMock = new Mock<IGenericRepo<Item>>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _mapperMock = new Mock<IMapper>();
            _loggerMock = new Mock<ILogger<ItemService>>();
            _itemService = new ItemService(_context,_mapperMock.Object,_genericRepoMock.Object,_unitOfWorkMock.Object,_loggerMock.Object);
        }
        [Fact]
        public async Task GetAllItems_ReturnAllItems()
        {
            var bookCategory = new Category
            {
                Id = 1,
                Name = "books",
                Description = "books category"
            };
            var newItems = new List<Item>
            {
                new Item{ Id = 1,Name="book 1",Price=100,StockQuantity=10,Category=bookCategory,CategoryId=bookCategory.Id},
                new Item{ Id = 2,Name="book 2",Price=200,StockQuantity=20,Category=bookCategory,CategoryId=bookCategory.Id},
            };
            await _context.Categories.AddAsync(bookCategory);
            await _context.AddRangeAsync(newItems);
            await _context.SaveChangesAsync();
            var newItemsDto = new List<ItemDto>
            {
                new ItemDto{Name="book 1",Price=100,StockQuantity=10,CategoryName=bookCategory.Name},
                new ItemDto{Name="book 2",Price=200,StockQuantity=20,CategoryName=bookCategory.Name},
            };
            _mapperMock.Setup(a => a.Map<List<ItemDto>>(It.IsAny<List<Item>>())).Returns(newItemsDto);
            var result = await _itemService.getAllItems();
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
        }
        [Fact]
        public async Task GetItem_WithName_ReturnItem()
        {
            var category = new Category
            {
                Id = 1,
                Name = "Electronics",
                Description="Electronics description"
            };

            var item = new Item
            {
                Id = 1,
                Name = "Laptop",
                Price = 5000,
                StockQuantity = 10,
                Category = category,
                
            };

            await _context.Categories.AddAsync(category);
            await _context.Items.AddAsync(item);
            await _context.SaveChangesAsync();

            var itemDto = new ItemDto
            {
                Name = "Laptop",
                Price = 5000,
                StockQuantity = 10
            };

            _mapperMock.Setup(x => x.Map<ItemDto>(It.IsAny<Item>())).Returns(itemDto);

            // Act
            var result = await _itemService.getITem("Laptop");

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Laptop",result.Name);

        }
        [Fact]
        public async Task GetItems_ByCategoryName_ReturnItems()
        {
            var bookCategory = new Category
            {
                Id = 1,
                Name = "books",
                Description = "books category"
            };
            
            var carCategory = new Category
            {
                Id = 2,
                Name = "cars",
                Description = "cars category"
            };

            var newItems = new List<Item>
            {
                new Item{ Id = 1,Name="book 1",Price=100,StockQuantity=10,Category=bookCategory,CategoryId=bookCategory.Id},
                new Item{ Id = 2,Name="book 2",Price=200,StockQuantity=20,Category=bookCategory,CategoryId=bookCategory.Id},
                new Item{ Id = 3,Name="car 1",Price=5000,StockQuantity=30,Category=carCategory,CategoryId=carCategory.Id},
            };
            
            await _context.Categories.AddAsync(bookCategory);
            await _context.AddRangeAsync(newItems);
            await _context.SaveChangesAsync();
            
            var newItemsDto = new List<ItemDto>
            {
                new ItemDto{Name="book 1",Price=100,StockQuantity=10,CategoryName=bookCategory.Name},
                new ItemDto{Name="book 2",Price=200,StockQuantity=20,CategoryName=bookCategory.Name},
                //new ItemDto{Name="car 1",Price=5000,StockQuantity=30,CategoryName=carCategory.Name},
            };

            _mapperMock.Setup(a => a.Map<List<ItemDto>>(It.IsAny<List<Item>>())).Returns(newItemsDto);

            var result = await _itemService.getITemByCategoryName("books",2,1);
            Assert.NotNull(result);
            Assert.All(result,a=> Assert.Equal("books",a.CategoryName)); 

        }
    }
}
