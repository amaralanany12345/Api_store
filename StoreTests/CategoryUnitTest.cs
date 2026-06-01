using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using StoreService.DTO;
using StoreService.Interfaces;
using StoreDomain.Models;
using StoreService.Services;
using StoreDataBase.AppContexts;
using StoreService.ResponseModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoreTests
{
    public class CategoryServiceTests
    {
        private readonly Mock<IUnitOfWorkServiceForStoreDb> _unitOfWorkMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ILogger<CategoryService>> _loggerMock;
        private readonly AppDbContext _context;
        private readonly CategoryService _categoryService;
        public CategoryServiceTests()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
            _context = new AppDbContext(options);
            _unitOfWorkMock = new Mock<IUnitOfWorkServiceForStoreDb>();
            _mapperMock = new Mock<IMapper>();
            _loggerMock = new Mock<ILogger<CategoryService>>();
            _categoryService = new CategoryService(_mapperMock.Object, _loggerMock.Object, _unitOfWorkMock.Object);
        }

        [Fact]
        public async Task createCategory_withCategoryName_ReturnCategory()
        {
            var newCategory = new Category
            {
                Id=1,
                Name="books",
                Description="books category",
            };
            var newCategoryDto = new CategoryDto
            {
                Name = "books",
                Description = "books category"
            };
            _unitOfWorkMock.Setup(a => a.Categories.CreateAsync(newCategory)).Returns(Task.CompletedTask);
            _mapperMock.Setup(a=>a.Map<CategoryDto>(It.IsAny<Category>())).Returns(newCategoryDto);
            var result = await _categoryService.CreateCategory(newCategory.Name, newCategory.Description);
            Assert.NotNull(result);
            Assert.Equal(newCategory.Name,result.Result.Name);
        }
        [Fact]
        public async Task DeleteCategory_ByCategoryId_Deleted()
        {
            var newCategory = new Category
            {
                Id = 1,
                Name = "books",
                Description = "books category",
            };
            await _context.Categories.AddAsync(newCategory);
            await _context.SaveChangesAsync();
            _unitOfWorkMock.Setup(a => a.Categories.GetAsync(newCategory.Id)).ReturnsAsync(newCategory);
            _unitOfWorkMock.Setup(a => a.Categories.DeleteAsync(newCategory.Id)).Returns(Task.CompletedTask);
            await _categoryService.DeleteCategory(newCategory.Id);
            _unitOfWorkMock.Verify(a => a.Categories.DeleteAsync(newCategory.Id));
        }
        [Fact]
        public async Task GetAllCategories_ReturnAllCategories()
        {
            var newCategories = new List<Category>
            {
                new Category{Id = 1,Name = "books",Description = "books category" },
                new Category{Id=2, Name="electronics",Description="electronics category"},
            };
            await _context.Categories.AddRangeAsync(newCategories);
            await _context.SaveChangesAsync();
            var newCategoriesDto = new List<CategoryDto>
            {
                new CategoryDto{Name = "books",Description = "books category" },
                new CategoryDto{Name="electronics",Description="electronics category"},
            };
            _unitOfWorkMock.Setup(a => a.Categories.GetAllAsync()).ReturnsAsync(newCategories);
            _mapperMock.Setup(a=>a.Map<List<CategoryDto>>(It.IsAny<List<Category>>())).Returns(newCategoriesDto);
            var result = await _categoryService.GetAllCategories();
            Assert.Equal(2, result.Result.Count);
        }
        [Fact]
        public async Task getCategory_ById_ReturnCategory()
        {
            var newCategory = new Category
            {
                Id = 1,
                Name = "books",
                Description = "books category",
            };
            await _context.Categories.AddAsync(newCategory);
            await _context.SaveChangesAsync();
            var newCategoryDto = new CategoryDto
            {
                Name = "books",
                Description = "books category"
            };
            _unitOfWorkMock.Setup(a => a.Categories.GetAsync(newCategory.Id)).ReturnsAsync(newCategory);
            _mapperMock.Setup(a=>a.Map<CategoryDto>(It.IsAny<Category>())).Returns(newCategoryDto);
            var result= await _categoryService.GetCategory(newCategory.Id);
            Assert.NotNull(result);
            Assert.Equal("books category",result.Result.Description);
            Assert.Equal("books",result.Result.Name);
        }
        [Fact]
        public async Task UpdateCategory_ById_ReturnUpdatedCategory()
        {
            var newCategory = new Category
            {
                Id = 1,
                Name = "books",
                Description = "books category",
            };
            await _context.Categories.AddAsync(newCategory);
            await _context.SaveChangesAsync();
            var newUpdatedCategoryDto = new CategoryDto
            {
                Name = "cars",
                Description = "cars category"
            };
            _unitOfWorkMock.Setup(a => a.Categories.GetAsync(newCategory.Id)).ReturnsAsync(newCategory);
            _mapperMock.Setup(a=>a.Map<CategoryDto>(It.IsAny<Category>())).Returns(newUpdatedCategoryDto);
            var result=await _categoryService.UpdateCategory(newCategory.Id,newUpdatedCategoryDto.Name
                ,newUpdatedCategoryDto.Description);
            Assert.NotNull(result);
            Assert.Equal("cars", result.Result.Name);
            Assert.Equal("cars category", result.Result.Description);
        }
    }

}
