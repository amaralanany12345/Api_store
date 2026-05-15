using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Serilog;
using StoreWebApi.DTO;
using StoreWebApi.Enums;
using StoreWebApi.Interfaces;
using StoreWebApi.Models;
using StoreWebApi.zAppContexts;

namespace StoreWebApi.Services
{
    public class OrderService : IOrder
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        private readonly IGenericRepo<Order> _genericRepo;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<OrderService> _logger;
        private readonly IUser _userService;
        public OrderService(AppDbContext context, IMapper mapper, IGenericRepo<Order> genericRepo, IUnitOfWork unitOfWork, ILogger<OrderService> logger, IUser userService)
        {
            _context = context;
            _mapper = mapper;
            _genericRepo = genericRepo;
            _unitOfWork = unitOfWork;
            _logger = logger;
            _userService = userService;
        }

        public async Task<OrderDto> createOrder()
        {
            var currentCustomer= await _userService.getCurrentUser();
            var customer=await _context.Users.Where(a=>a.Id==currentCustomer.Id && a.Role==UserRole.Customer.ToString()).FirstOrDefaultAsync();
            if(customer == null)
            {
                _logger.LogWarning("customer is not found so you cannot make order");
                throw new ArgumentException("customer is not found");
            }
            var newOrder=new Order
            {
                CustomerId=currentCustomer.Id,
                Customer=customer,
                CreatedAt=DateTime.Now,
                Status=OrderStatus.InProgress.ToString(),
                TotalAmount=0,
            };
            await _genericRepo.CreateAsync(newOrder);
            await _unitOfWork.saveChangesAsync();
            _logger.LogInformation($"new order is created with user email is {customer.Email}");
            return _mapper.Map<OrderDto>(newOrder);
        }

        public async Task<OrderItem> AddOrderItemToOrder(int itemId, int quantity)
        {
            var order = await getOrder();
            var item = await _context.Items.Where(a => a.Id == itemId).FirstOrDefaultAsync();
            if (item == null)
            {
                _logger.LogWarning("item is not found so you cannot add it to your order");
                throw new ArgumentException("item is not found");
            }
            if (quantity > item.StockQuantity)
            {
                _logger.LogWarning("the stock quantity is not enough");
                throw new ArgumentException("the stock quantity is not enough");
            }
            var newOrderITem = new OrderItem
            {
                Order = order,
                OrderId = order.Id,
                Item = item,
                ItemId = itemId,
                Quantity = quantity
            };
            order.Status = OrderStatus.InProgress.ToString();
            order.TotalAmount += quantity * item.Price;
            order.UpdatedAt = DateTime.Now;
            item.StockQuantity -= newOrderITem.Quantity;
            await _context.OrderItem.AddAsync(newOrderITem);
            await _context.SaveChangesAsync();
            return newOrderITem;
        }

        public async Task<List<OrderDto>> getAllOrders()
        {
            _logger.LogInformation("all order are retrieved");
            return _mapper.Map<List<OrderDto>>(await _context.Orders.ToListAsync());
        }

        public async Task<Order> getOrder()
        {
            var customer = await _userService.getCurrentUser();
            var order=await _context.Orders.Where(a=>a.CustomerId == customer.Id&& a.Status==OrderStatus.InProgress.ToString()).Include(a=>a.Customer).Include(a=>a.OrderItems).FirstOrDefaultAsync();
            if(order == null)
            {
                _logger.LogWarning("order is not found");
                throw new ArgumentException("order is not found");
            }
            return order;
        }

        public async Task deleteOrderItemFromOrder(int itemId)
        {
            var order = await getOrder();
            var item = await _context.Items.Where(a => a.Id == itemId).FirstOrDefaultAsync();
            if (item == null)
            {
                _logger.LogWarning("item is not found");
                throw new ArgumentException("item is not found");
            }
            var orderItem = await _context.OrderItem.Where(a => a.OrderId == order.Id && a.ItemId == itemId).FirstOrDefaultAsync();
            if (orderItem == null)
            {
                _logger.LogWarning("order item is not found");
                throw new ArgumentException("order item is not found");
            }
            order.Status = OrderStatus.InProgress.ToString();
            order.TotalAmount -= orderItem.Quantity * item.Price;
            order.UpdatedAt = DateTime.Now;
            item.StockQuantity += orderItem.Quantity;
            _context.OrderItem.Remove(orderItem);
            await _context.SaveChangesAsync();
        }

        public async Task<List<OrderItem>> getOrderItems()
        {
            var order = await getOrder();
            var orderItems=await _context.OrderItem.Where(a=>a.OrderId==order.Id).Include(a=>a.Item).ToListAsync();
            return orderItems;
        }

        public async Task CancelOrder()
        {
            var order = await getOrder();
            order.TotalAmount = 0;
            order.UpdatedAt = DateTime.Now;
            order.Status = OrderStatus.Cancelled.ToString();
            var orderItems=await getOrderItems();
            foreach(var orderItem in orderItems)
            {
                 orderItem.Item.StockQuantity+=orderItem.Quantity;
                _context.OrderItem.Remove(orderItem);
                await _context.SaveChangesAsync();
            }
            await _context.SaveChangesAsync();
        }

        public async Task<List<OrderItemDto>> getOrderItemsById(int orderId)
        {
            return _mapper.Map<List<OrderItemDto>>(await _context.OrderItem.Where(a=>a.OrderId==orderId).Include(a=>a.Item).ToListAsync());
        }
    }
}
