using AutoMapper;
using StoreDomain.Enums;
using StoreService.Interfaces;
using StoreDomain.Models;
using StoreService.DTO;
using Microsoft.Extensions.Logging;

namespace StoreService.Services
{
    public class OrderService : IOrderService
    {
        private readonly IMapper _mapper;
       
        private readonly IUnitOfWorkServiceForStoreDb _unitOfWork;
        private readonly ILogger<OrderService> _logger;
        private readonly IUserService _userService;
        public OrderService( IMapper mapper, IUnitOfWorkServiceForStoreDb unitOfWork, ILogger<OrderService> logger, IUserService userService)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _logger = logger;
            _userService = userService;
        }

        public async Task<OrderDto> CreateOrder()
        {
            var currentCustomer= await _userService.GetCurrentUser();
            var customer=await _unitOfWork.Users.GetFirstOrDefault(a=>a.Id==currentCustomer.Id && a.Role==UserRole.Customer.ToString());
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
            await _unitOfWork.Orders.CreateAsync(newOrder);
            await _unitOfWork.SaveChangesAsync();
            _logger.LogInformation($"new order is created with user email is {customer.Email}");
            return _mapper.Map<OrderDto>(newOrder);
        }

        public async Task<OrderItem> AddOrderItemToOrder(int itemId, int quantity)
        {
            var order = await GetOrder();
            var item = await _unitOfWork.Items.GetAsync(itemId);
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
            await _unitOfWork.OrderItems.CreateAsync(newOrderITem);
            await _unitOfWork.SaveChangesAsync();
            return newOrderITem;
        }

        public async Task<List<OrderDto>> GetAllOrders()
        {
            _logger.LogInformation("all order are retrieved");
            return _mapper.Map<List<OrderDto>>(await _unitOfWork.Orders.GetAllAsync());
        }

        public async Task<Order> GetOrder()
        {
            var customer = await _userService.GetCurrentUser();
            var order = await _unitOfWork.OrderRepository.GetOrder(customer.Id);
            return order;
        }

        public async Task DeleteOrderItemFromOrder(int itemId)
        {
            var order = await GetOrder();
            var item = await _unitOfWork.Items.GetFirstOrDefault(a => a.Id == itemId);
            if (item == null)
            {
                _logger.LogWarning("item is not found");
                throw new ArgumentException("item is not found");
            }
            var orderItem = await _unitOfWork.OrderItems.GetFirstOrDefault(a => a.OrderId == order.Id && a.ItemId == itemId);
            if (orderItem == null)
            {
                _logger.LogWarning("order item is not found");
                throw new ArgumentException("order item is not found");
            }
            order.Status = OrderStatus.InProgress.ToString();
            order.TotalAmount -= orderItem.Quantity * item.Price;
            order.UpdatedAt = DateTime.Now;
            item.StockQuantity += orderItem.Quantity;
            _unitOfWork.OrderItems.DeleteAsync(orderItem);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<List<OrderItem>> GetOrderItems()
        {
            var order = await GetOrder();
            var orderItems = await _unitOfWork.OrderRepository.GetOrderItems(order.Id);
            return orderItems;
        }

        public async Task CancelOrder()
        {
            var order = await GetOrder();
            order.TotalAmount = 0;
            order.UpdatedAt = DateTime.Now;
            order.Status = OrderStatus.Cancelled.ToString();
            var orderItems=await GetOrderItems();
            foreach(var orderItem in orderItems)
            {
                 orderItem.Item.StockQuantity+=orderItem.Quantity;
                _unitOfWork.OrderItems.DeleteAsync(orderItem);
                await _unitOfWork.SaveChangesAsync();
            }
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<List<OrderItemDto>> GetOrderItemsById(int orderId)
        {
            return _mapper.Map<List<OrderItemDto>>(await _unitOfWork.OrderRepository.GetOrderItemsById(orderId));
        }
    }
}
