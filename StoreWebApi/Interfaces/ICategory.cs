using StoreWebApi.DTO;
using StoreWebApi.Models;

namespace StoreWebApi.Interfaces
{
    public interface ICategory
    {
        Task<CategoryDto> createCategory(string name,string description);
        Task<List<CategoryDto>> getAllCategories();
        Task<CategoryDto> getCategory(string name);
        Task<CategoryDto> updateCategory(string CategoryName, string newName,string newDescription);
        Task deleteCategory(string CategoryName);
    }
}
