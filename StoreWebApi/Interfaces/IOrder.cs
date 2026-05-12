using StoreWebApi.DTO;
using StoreWebApi.Models;

namespace StoreWebApi.Interfaces
{
    public interface IOrder
    {
        Task<OrderDto> createOrder();
        Task<List<OrderDto>> getAllOrders();
        Task<OrderItem> AddOrderItemToOrder(int itemId,int quantity);
        Task deleteOrderItemFromOrder(int itemId);
        Task<Order> getOrder();
        Task<List<OrderItem>> getOrderItems();
        Task CancelOrder();
    }
}
