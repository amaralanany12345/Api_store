using StoreService.DTO;
using StoreService.ResponseModel;

namespace StoreService.Interfaces
{
    public interface ICategoryService
    {
        Task<ResultResponse<CategoryDto>> CreateCategory(string name,string description);
        Task<ResultResponse<List<CategoryDto>>> GetAllCategories();
        Task<ResultResponse<CategoryDto>> GetCategory(int categoryId);
        Task<ResultResponse<CategoryDto>> UpdateCategory(int categoryId, string newName,string newDescription);
        Task DeleteCategory(int categoryId);
    }
}
