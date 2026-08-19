using StoreService.DTO;
using StoreDomain.Models;
using StoreService.ResponseModel;

namespace StoreService.Interfaces
{
    public interface IOrderService
    {
        Task<ResultResponse<OrderDto>> CreateOrder();
        Task<ResultResponse<List<OrderDto>>> GetAllOrders();
        Task<ResultResponse<OrderItemDto>> AddOrderItemToOrder(OrderItemDto orderItemDto);
        Task<ResultResponse<OrderItemDto>> IncreaseQuantityOfItem(OrderItemDto orderItemDto);
        Task<ResultResponse<OrderItemDto>> DecreaseQuantityOfItem(OrderItemDto orderItemDto);
        Task DeleteOrderItemFromOrder(int itemId);
        Task<ResultResponse<Order>> GetOrder();
        Task<ResultResponse<List<OrderItem>>> GetOrderItems();
        Task<ResultResponse<List<OrderItemDto>>> GetOrderItemsById(int orderId);
        Task CancelOrder();
    }
}
