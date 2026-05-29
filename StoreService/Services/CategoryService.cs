using AutoMapper;
using AutoMapper;
using Serilog;
using StoreDomain.Enums;
using StoreService.DTO;
using StoreService.Interfaces;
using StoreDomain.Models;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
using System.Xml.Linq;
using Microsoft.AspNetCore.Http;
using StoreService.ResponseModel;
namespace StoreService.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly IMapper _mapper;
        private readonly ILogger<CategoryService> _logger;
        private readonly IUnitOfWorkServiceForStoreDb _unitOfWork;
        public CategoryService(IMapper mapper, ILogger<CategoryService> logger, IUnitOfWorkServiceForStoreDb unitOfWork)
        {
            _mapper = mapper;
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public async Task<ResultResponse<CategoryDto>> CreateCategory(string name, string description)
        { 
            var newCategory = new Category { Name = name, Description = description };
            var existCategory=await _unitOfWork.Categories.GetFirstOrDefault(a=>a.Name == newCategory.Name);
            if (existCategory != null)
            {
                return ResultResponse<CategoryDto>.Fail("category is already exist",ErrorTypes.Conflict,StatusCodes.Status409Conflict);
            }
            await _unitOfWork.Categories.CreateAsync(newCategory);
            await _unitOfWork.SaveChangesAsync();
            _logger.LogInformation($"category is created with name {name}");
            return ResultResponse<CategoryDto>.Pass(_mapper.Map<CategoryDto>(newCategory),StatusCodes.Status201Created);
        }

        public async Task DeleteCategory(int categoryId)
        {
            var category=await _unitOfWork.Categories.GetAsync(categoryId);
            if(category == null)
            {
                ResultResponse<Category>.Fail("category is not found",ErrorTypes.NotFound,StatusCodes.Status404NotFound);
                return;
            }
            await _unitOfWork.Categories.DeleteAsync(categoryId);
            _logger.LogInformation($"category is deleted");
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<ResultResponse<List<CategoryDto>>> GetAllCategories()
        {
            _logger.LogInformation("all categories are retrieved");
            var allCategories = await _unitOfWork.Categories.GetAllAsync();
            return ResultResponse<List<CategoryDto>>.Pass(_mapper.Map<List<CategoryDto>>(allCategories),StatusCodes.Status200OK);
        }

        public async Task<ResultResponse<CategoryDto>> GetCategory(int categoryId)
        {
            var category=await _unitOfWork.Categories.GetAsync(categoryId);
            if (category == null)
            {
                return ResultResponse<CategoryDto>.Fail("category is not found",ErrorTypes.NotFound, StatusCodes.Status404NotFound);
            }
            return ResultResponse<CategoryDto>.Pass(_mapper.Map<CategoryDto>(category),StatusCodes.Status200OK);
        }

        public async Task<ResultResponse<CategoryDto>> UpdateCategory(int categoryId, string newName, string newDescription)
        {
            var category=await _unitOfWork.Categories.GetAsync(categoryId);
            if (category == null)
            {
                return ResultResponse<CategoryDto>.Fail("category is not found", ErrorTypes.NotFound, StatusCodes.Status404NotFound);
            }
            category.Name = newName;
            category.Description = newDescription;
            await _unitOfWork.SaveChangesAsync();
            _logger.LogInformation($"category is Updated with name :{newName}");
            return ResultResponse<CategoryDto>.Pass(_mapper.Map<CategoryDto>(category), StatusCodes.Status200OK);
        }

    }
}
