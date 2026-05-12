using StoreWebApi.DTO;
using StoreWebApi.Models;

namespace StoreWebApi.Interfaces
{
    public interface IItem
    {
        Task<ItemDto> createItem(string name, int price, int stockQuantity,string categoryName);
        Task<List<ItemDto>> getAllItems();
        Task<ItemDto> getITem(string name);
        Task<ItemDto> updateItem(string itemName, string newName, int newPrice,int stockQuantity);
        Task deleteItem(string itemName);
        Task<List<ItemDto>> getITemByCategoryName(string categoryName);
    }
}
