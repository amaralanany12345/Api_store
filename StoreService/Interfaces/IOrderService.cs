using StoreService.DTO;
using StoreDomain.Models;

namespace StoreService.Interfaces
{
    public interface IOrderService
    {
        Task<OrderDto> CreateOrder();
        Task<List<OrderDto>> GetAllOrders();
        Task<OrderItem> AddOrderItemToOrder(int itemId,int quantity);
        Task DeleteOrderItemFromOrder(int itemId);
        Task<Order> GetOrder();
        Task<List<OrderItem>> GetOrderItems();
        Task<List<OrderItemDto>> GetOrderItemsById(int orderId);
        Task CancelOrder();
    }
}
