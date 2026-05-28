using StoreService.DTO;

namespace StoreService.Interfaces
{
    public interface IItemService
    {
        Task<ItemDto> CreateItem(string name, int price, int stockQuantity,string categoryName);
        Task<List<ItemDto>> GetAllItems();
        Task<ItemDto> GetITem(int itemId);
        Task<ItemDto> UpdateItem(int itemId, string newName, int newPrice,int stockQuantity);
        Task DeleteItem(int itemId);
        Task<List<ItemDto>> GetITemByCategory(int categoryId, int pageSize, int pageNumber);
    }
}
