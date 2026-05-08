using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Serilog;
using StoreWebApi.DTO;
using StoreWebApi.Interfaces;
using StoreWebApi.Models;

namespace StoreWebApi.Services
{
    public class CategoryService : ICategory
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        public CategoryService(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<CategoryDto> createCategory(string name, string description)
        {
            var newCategory = new Category { Name = name, Description = description };
            await _context.Categories.AddAsync(newCategory);
            await _context.SaveChangesAsync();
            return _mapper.Map<CategoryDto>(newCategory);
        }

        public async Task deleteCategory(int CategoryId)
        {
            var category = await getCategoryById(CategoryId);
            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
        }
        private async Task<Category> getCategoryById(int categoryId)
        {
            var category = await _context.Categories.Where(a => a.Id == categoryId).FirstOrDefaultAsync();
            if (category == null)
            {
                throw new ArgumentException("category is not found");
            }
            return category;
        }

        public async Task<List<CategoryDto>> getAllCategories()
        {
            return _mapper.Map<List<CategoryDto>>(await _context.Categories.ToListAsync());
        }

        public async Task<CategoryDto> getCategory(string name)
        {
            var category=await _context.Categories.Where(a=>a.Name== name).FirstOrDefaultAsync();
            if(category == null)
            {
                throw new ArgumentException("category is not found");
            }
            return _mapper.Map<CategoryDto>(category);
        }

        public async Task<CategoryDto> updateCategory(int CategoryId, string newName, string newDescription)
        {
            var category=await getCategoryById(CategoryId);
            category.Name = newName;
            category.Description = newDescription;
            await _context.SaveChangesAsync();
            return _mapper.Map<CategoryDto>(category);
        }
    }
}
