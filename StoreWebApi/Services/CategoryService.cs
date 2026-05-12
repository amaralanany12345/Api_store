using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Serilog;
using StoreWebApi.DTO;
using StoreWebApi.Interfaces;
using StoreWebApi.Models;
using StoreWebApi.zAppContexts;

namespace StoreWebApi.Services
{
    public class CategoryService : ICategory
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<CategoryService> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IGenericRepo<Category> _genericRepo;
        public CategoryService(AppDbContext context, IMapper mapper, ILogger<CategoryService> logger, IUnitOfWork unitOfWork, IGenericRepo<Category> genericRepo)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
            _unitOfWork = unitOfWork;
            _genericRepo = genericRepo;
        }

        public async Task<CategoryDto> createCategory(string name, string description)
        {
            var newCategory = new Category { Name = name, Description = description };
            await _genericRepo.CreateAsync(newCategory);
            await _unitOfWork.saveChangesAsync();
            _logger.LogInformation($"category is created with name {name}");
            return _mapper.Map<CategoryDto>(newCategory);
        }

        public async Task deleteCategory(string CategoryName)
        {
            var category = await getCategory(CategoryName);
            _context.Categories.Remove(_mapper.Map<Category>(category));
            _logger.LogInformation($"{category.Name} is deleted");
            await _context.SaveChangesAsync();
        }
        private async Task<Category> getCategoryById(int categoryId)
        {
            var category = await _context.Categories.Where(a => a.Id == categoryId).FirstOrDefaultAsync();
            if (category == null)
            {
                _logger.LogWarning("category is not found with this id");
                throw new ArgumentException("category is not found");
            }
            return category;
        }

        public async Task<List<CategoryDto>> getAllCategories()
        {
            _logger.LogInformation("all categories are retrieved");
            Console.WriteLine("all categories");

            return _mapper.Map<List<CategoryDto>>(await _context.Categories.ToListAsync());
        }

        public async Task<CategoryDto> getCategory(string name)
        {
            var category=await _context.Categories.Where(a=>a.Name== name).FirstOrDefaultAsync();
            if(category == null)
            {
                _logger.LogWarning("category is not found");
                throw new ArgumentException("category is not found");
            }
            return _mapper.Map<CategoryDto>(category);
        }

        public async Task<CategoryDto> updateCategory(string CategoryName, string newName, string newDescription)
        {
            var category=await getCategory(CategoryName);
            category.Name = newName;
            category.Description = newDescription;
            await _context.SaveChangesAsync();
            _logger.LogInformation($"category is Updated with name :{newName}");
            return _mapper.Map<CategoryDto>(category);
        }
    }
}
