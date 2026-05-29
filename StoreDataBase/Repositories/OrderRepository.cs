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

        public async Task DeleteOrderItem(int orderId, int itemId)
        {
            var orderItem=await _context.OrderItem.Where(a=>a.OrderId==orderId && a.ItemId==itemId).FirstOrDefaultAsync();
            if(orderItem == null)
            {
                throw new ArgumentException("order item is not found");
            }
            _context.OrderItem.Remove(orderItem);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteOrderItems(int orderId)
        {
            var orderItems=await GetOrderItems(orderId);
            foreach (var orderItem in orderItems)
            {
                orderItem.Item.StockQuantity += orderItem.Quantity;
                _context.OrderItem.Remove(orderItem);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<Order> GetOrder(int customerId)
        {
            var order = await _context.Orders.Where(a => a.CustomerId == customerId && a.Status == OrderStatus.InProgress.ToString())
                .Include(a => a.Customer).Include(a => a.OrderItems).FirstOrDefaultAsync();
            return order;
        }

        public async Task<List<OrderItem>> GetOrderItems(int orderId)
        {
            var orderItems = await _context.OrderItem.Where(a => a.OrderId == orderId).Include(a => a.Item).ToListAsync();
            return orderItems;
        }

        public async Task<List<OrderItem>> GetOrderItemsById(int orderId)
        {
            return await _context.OrderItem.Where(a => a.OrderId == orderId).Include(a => a.Item).ToListAsync();
        }
    }
}
