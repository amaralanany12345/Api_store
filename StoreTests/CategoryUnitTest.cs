using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using StoreWebApi.DTO;
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
    public class CategoryServiceTests
    {
        private readonly Mock<IGenericRepo<Category>> _genericRepoMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ILogger<CategoryService>> _loggerMock;
        private readonly AppDbContext _context;
        private readonly CategoryService _categoryService;
        public CategoryServiceTests()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
            _context = new AppDbContext(options);
            _genericRepoMock = new Mock<IGenericRepo<Category>>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _mapperMock = new Mock<IMapper>();
            _loggerMock = new Mock<ILogger<CategoryService>>();
            _categoryService = new CategoryService(_context, _mapperMock.Object, _loggerMock.Object, _unitOfWorkMock.Object, _genericRepoMock.Object);
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
            _mapperMock.Setup(a=>a.Map<CategoryDto>(It.IsAny<Category>())).Returns(newCategoryDto);
            var result = await _categoryService.createCategory(newCategory.Name, newCategory.Description);
            Assert.NotNull(result);
            Assert.Equal(newCategory.Name,result.Name);
        }
        [Fact]
        public async Task DeleteCategory_ByCategoryName_Deleted()
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
            _mapperMock.Setup(a=>a.Map<CategoryDto>(It.IsAny<CategoryDto>())).Returns(newCategoryDto);
            await _categoryService.deleteCategory(newCategory.Name);
            var deletedCategory=await _context.Categories.FirstOrDefaultAsync(a=>a.Name==newCategoryDto.Name);
            Assert.Null(deletedCategory);
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
            _mapperMock.Setup(a=>a.Map<List<CategoryDto>>(It.IsAny<List<Category>>())).Returns(newCategoriesDto);
            var result = await _categoryService.getAllCategories();
            Assert.Equal(2, result.Count);
        }
        [Fact]
        public async Task getCategory_ByName_ReturnCategory()
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
            _mapperMock.Setup(a=>a.Map<CategoryDto>(It.IsAny<Category>())).Returns(newCategoryDto);
            var result= await _categoryService.getCategory(newCategoryDto.Name);
            Assert.NotNull(result);
            Assert.Equal("books category",result.Description);
            Assert.Equal("books",result.Name);
        }
        [Fact]
        public async Task UpdateCategory_ByName_ReturnUpdatedCategory()
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
            _mapperMock.Setup(a=>a.Map<CategoryDto>(It.IsAny<Category>())).Returns(newUpdatedCategoryDto);
            var result=await _categoryService.updateCategory(newCategory.Name,newUpdatedCategoryDto.Name
                ,newUpdatedCategoryDto.Description);
            Assert.NotNull(result);
            Assert.Equal("cars", result.Name);
            Assert.Equal("cars category", result.Description);
        }
    }

}
