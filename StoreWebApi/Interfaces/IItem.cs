using StoreWebApi.DTO;
using StoreWebApi.Models;

namespace StoreWebApi.Interfaces
{
    public interface IItem
    {
        Task<ItemDto> createItem(string name, int price, int stockQuantity,int categoryId);
        Task<List<ItemDto>> getAllItems();
        Task<ItemDto> getITem(string name);
        Task<ItemDto> updateItem(int ItemId, string newName, int newPrice,int stockQuantity);
        Task deleteItem(int ItemId);
        Task<List<ItemDto>> getITemByCategoryName(string categoryName);
    }
}
