using StoreService.DTO;
using StoreService.ResponseModel;

namespace StoreService.Interfaces
{
    public interface IItemService
    {
        Task<ResultResponse<ItemDto>> CreateItem(string name, int price, int stockQuantity,string categoryName);
        Task<ResultResponse<List<ItemDto>>> GetAllItems();
        Task<ResultResponse<ItemDto>> GetITem(int itemId);
        Task<ResultResponse<ItemDto>> UpdateItem(int itemId, string newName, int newPrice,int stockQuantity);
        Task DeleteItem(int itemId);
        Task<ResultResponse<List<ItemDto>>> GetITemByCategory(int categoryId, int pageSize, int pageNumber);
        Task<ResultResponse<List<ItemDto>>> GetItemsByCategoryId(int categoryId);
        Task<ResultResponse<List<ItemDto>>> SearchByName(string itemName);
    }
}
