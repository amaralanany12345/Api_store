using StoreDomain.Models;
using StoreService.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoreService.RepositoriesInterfaces
{
    public interface IOrderRepository
    {
        Task<Order> GetOrder(int customerId);
        Task<List<OrderItem>> GetOrderItems(int orderId);
        Task<List<OrderItem>> GetOrderItemsById(int orderId);


    }
}
