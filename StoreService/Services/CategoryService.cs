using AutoMapper;
using Serilog;
using StoreService.DTO;
using StoreService.Interfaces;
using StoreDomain.Models;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
using System.Xml.Linq;
using StoreService.ResultPattern;
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
            await _unitOfWork.Categories.CreateAsync(newCategory);
            await _unitOfWork.SaveChangesAsync();
            _logger.LogInformation($"category is created with name {name}");
            return ResultResponse<CategoryDto>.Pass(_mapper.Map<CategoryDto>(newCategory));
        }

        public async Task DeleteCategory(int categoryId)
        {
            var category=await _unitOfWork.Categories.GetAsync(categoryId);
            if(category == null)
            {
                ResultResponse<Category>.Fail("category is not found",ErrorTypes.NotFound);
            }
            _unitOfWork.Categories.DeleteAsync(category);
            _logger.LogInformation($"category is deleted");
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<ResultResponse<List<CategoryDto>>> GetAllCategories()
        {
            _logger.LogInformation("all categories are retrieved");
            var allCategories = await _unitOfWork.Categories.GetAllAsync();
            return ResultResponse<List<CategoryDto>>.Pass(_mapper.Map<List<CategoryDto>>(allCategories));
        }

        public async Task<ResultResponse<CategoryDto>> GetCategory(int categoryId)
        {
            var category=await _unitOfWork.Categories.GetAsync(categoryId);
            if (category == null)
            {
                return ResultResponse<CategoryDto>.Fail("category is not found",ErrorTypes.NotFound);
            }
            return ResultResponse<CategoryDto>.Pass(_mapper.Map<CategoryDto>(category));
        }

        public async Task<ResultResponse<CategoryDto>> UpdateCategory(int categoryId, string newName, string newDescription)
        {
            var category=await _unitOfWork.Categories.GetAsync(categoryId);
            if (category == null)
            {
                return ResultResponse<CategoryDto>.Fail("category is not found", ErrorTypes.NotFound);
            }
            category.Name = newName;
            category.Description = newDescription;
            await _unitOfWork.SaveChangesAsync();
            _logger.LogInformation($"category is Updated with name :{newName}");
            return ResultResponse<CategoryDto>.Pass(_mapper.Map<CategoryDto>(category));
        }

    }
}
