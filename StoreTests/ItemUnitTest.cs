using AutoMapper;
using Castle.Core.Logging;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using StoreService.DTO;
using StoreService.Interfaces;
using StoreDomain.Models;
using StoreService.Services;
using StoreDataBase.AppContexts;
using System;
using System.Net.Http;
using System.Net;
using StoreService.RepositoriesInterfaces;

namespace StoreTests
{
    public class ItemServiceTests
    {
        private readonly Mock<IUnitOfWorkServiceForStoreDb> _unitOfWorkMock;
        private readonly Mock<IITemRepository> _itemRepositoryMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ILogger<ItemService>> _loggerMock;
        private readonly AppDbContext _context;
        private readonly IItemService _itemService;

        public ItemServiceTests()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
            _context = new AppDbContext(options);
            _unitOfWorkMock = new Mock<IUnitOfWorkServiceForStoreDb>();
            _mapperMock = new Mock<IMapper>();
            _loggerMock = new Mock<ILogger<ItemService>>();
            _itemRepositoryMock = new Mock<IITemRepository>();
            _itemService = new ItemService(_mapperMock.Object,_unitOfWorkMock.Object,_loggerMock.Object);
        }
        [Fact]
        public async Task UpdateITem_ByName_ReturnNewUpdatedItem()
        {
            var category = new Category
            {
                Id = 1,
                Name = "books",
                Description = "books category",
            };
            var newItem = new Item
            {
                Id = 1,
                Name = "item 1",
                Price = 100,
                StockQuantity = 20,
                Category = category,
                CategoryId = category.Id,
            };
            await _context.Categories.AddAsync(category);
            await _context.Items.AddAsync(newItem);
            await _context.SaveChangesAsync();
            var newItemDto = new ItemDto
            {
                Name = "item 2",
                Price = 200,
                StockQuantity = 30,
                CategoryName = category.Name
            };
            _mapperMock.Setup(a => a.Map<ItemDto>(It.IsAny<Item>())).Returns(newItemDto);
            var result = await _itemService.UpdateItem(newItem.Id,newItemDto.Name,newItemDto.Price,newItemDto.StockQuantity);
            Assert.NotNull(result);
            Assert.Equal(newItemDto.Name,result.Name);
            Assert.Equal(newItemDto.Price,result.Price);
        }
        [Fact]
        public async Task DeleteItem_byItemName_ReturnNull()
        {
            var category = new Category
            {
                Id = 1,
                Name = "books",
                Description = "books category",
            };
            var newItem = new Item
            {
                Id = 1,
                Name = "item 1",
                Price = 100,
                StockQuantity = 20,
                Category = category,
                CategoryId = category.Id,
            };
            await _context.Categories.AddAsync(category);
            await _context.Items.AddAsync(newItem);
            await _context.SaveChangesAsync();
            var itemDto = new ItemDto
            {
                Name = "item 1",
                Price = 100,
                StockQuantity = 20,
                CategoryName = category.Name
            };
            _unitOfWorkMock.Setup(a => a.Items.DeleteAsync(newItem)).Returns(Task.CompletedTask);
            _mapperMock.Setup(a=>a.Map<ItemDto>(It.IsAny<Item>())).Returns(itemDto);

            await _itemService.DeleteItem(newItem.Id);
            var deletedITem=await _context.Items.FirstOrDefaultAsync(a=>a.Name=="itemDto.Name");
            Assert.Null(deletedITem);
        }
        [Fact]
        public async Task CreateItem_withCategoryName_ReturnItem()
        {
            var category = new Category
            {
                Id=1,
                Name="books",
                Description="books category",
            };
            var newItem = new Item
            {
                Id=1,
                Name="item 1",
                Price=100,
                StockQuantity=20,
                Category=category,
                CategoryId=category.Id,
            };
            await _context.Categories.AddAsync(category);
            await _context.SaveChangesAsync();
            _unitOfWorkMock.Setup(a => a.Categories.CreateAsync(category)).Returns(Task.CompletedTask);
            var newITemDto = new ItemDto
            {
                Name = "item 1",
                Price = 100,
                StockQuantity = 20,
                CategoryName=category.Name,
            };
            _unitOfWorkMock.Setup(a => a.Items.CreateAsync(newItem)).Returns(Task.CompletedTask);
            _mapperMock.Setup(a => a.Map<ItemDto>(It.IsAny<Item>())).Returns(newITemDto);
            var result = await _itemService.CreateItem(newITemDto.Name,newITemDto.Price,newITemDto.StockQuantity,newITemDto.CategoryName);
            Assert.NotNull(newItem);
            Assert.Equal(newItem.Name,result.Name);
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
            _unitOfWorkMock.Setup(a => a.Items.GetAllAsync()).ReturnsAsync(newItems);
            _mapperMock.Setup(a => a.Map<List<ItemDto>>(It.IsAny<List<Item>>())).Returns(newItemsDto);
            var result = await _itemService.GetAllItems();
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
        }
        [Fact]
        public async Task GetItem_WithId_ReturnItem()
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
            _unitOfWorkMock.Setup(a => a.Items.GetAsync(item.Id)).ReturnsAsync(item);
            _mapperMock.Setup(x => x.Map<ItemDto>(It.IsAny<Item>())).Returns(itemDto);

            // Act
            var result = await _itemService.GetITem(1);

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
            _itemRepositoryMock.Setup(a=>a.GetITemByCategory(bookCategory.Id,2,1)).ReturnsAsync(newItems);
            _mapperMock.Setup(a => a.Map<List<ItemDto>>(It.IsAny<List<Item>>())).Returns(newItemsDto);

            var result = await _itemService.GetITemByCategory(bookCategory.Id,2,1);
            Assert.NotNull(result);
            Assert.All(result,a=> Assert.Equal("books",a.CategoryName)); 

        }
    }
}
