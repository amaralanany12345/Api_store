using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using StoreDataBase.AppContexts;
using StoreDomain.Enums;
using StoreDomain.Models;
using StoreService.DTO;
using StoreService.RepositoriesInterfaces;
namespace StoreDataBase.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly AppDbContext _context;
        public OrderRepository(AppDbContext context) 
        {
            _context = context;
        }

        public async Task<Order> GetOrder(int customerId)
        {
            //var orders = await _context.Orders.Where();
            var order = await _context.Orders.Where(a => a.CustomerId == customerId && a.Status == OrderStatus.InProgress.ToString()).Include(a => a.Customer).Include(a => a.OrderItems).FirstOrDefaultAsync();
            if (order == null)
            {
                throw new ArgumentException("order is not found");
            }
            return order;
        }

        public async Task<List<OrderItem>> GetOrderItems(int orderId)
        {
            var order = await GetOrder(orderId);
            var orderItems = await _context.OrderItem.Where(a => a.OrderId == orderId).Include(a => a.Item).ToListAsync();
            return orderItems;
        }

        public async Task<List<OrderItem>> GetOrderItemsById(int orderId)
        {
            return await _context.OrderItem.Where(a => a.OrderId == orderId).Include(a => a.Item).ToListAsync();
        }
    }
}
