using AutoMapper;
using StoreDomain.Enums;
using StoreService.Interfaces;
using StoreDomain.Models;
using StoreService.DTO;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using StoreService.ResponseModel;

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

        public async Task<ResultResponse<OrderDto>> CreateOrder()
        {
            var currentCustomer= await _userService.GetCurrentUser();
            var customer=await _unitOfWork.Users.GetFirstOrDefault(a=>a.Id==currentCustomer.Result.Id && a.Role==UserRole.Customer.ToString());
            if(customer == null)
            {
                _logger.LogWarning("customer is not found so you cannot make order");
                return ResultResponse<OrderDto>.Fail("customer is not found so you cannot make order", ErrorTypes.NotFound, StatusCodes.Status404NotFound);
            }
            var newOrder=new Order
            {
                CustomerId=currentCustomer.Result.Id,
                Customer=customer,
                CreatedAt=DateTime.Now,
                Status=OrderStatus.InProgress.ToString(),
                TotalAmount=0,
            };
            await _unitOfWork.Orders.CreateAsync(newOrder);
            await _unitOfWork.SaveChangesAsync();
            _logger.LogInformation($"new order is created with user email is {customer.Email}");
            return ResultResponse<OrderDto>.Pass(_mapper.Map<OrderDto>(newOrder),StatusCodes.Status201Created);
        }

        public async Task<ResultResponse<OrderItemDto>> AddOrderItemToOrder(OrderItemDto orderItemDto)
        {
            var order = await GetOrder();
            var item = await _unitOfWork.Items.GetAsync(orderItemDto.ItemId);
            if (item == null)
            {
                _logger.LogWarning("item is not found so you cannot add it to your order");
                return ResultResponse<OrderItemDto>.Fail("item is not found so you cannot add it to your order", ErrorTypes.NotFound, StatusCodes.Status404NotFound);
            }
            if (orderItemDto.Quantity> item.StockQuantity)
            {
                _logger.LogWarning("the stock quantity is not enough");
                return ResultResponse<OrderItemDto>.Fail("the stock quantity is not enough", ErrorTypes.BadRequest, StatusCodes.Status400BadRequest);
            }
            var newOrderITem = new OrderItem
            {
                Order = order.Result,
                OrderId = order.Result.Id,
                Item = item,
                ItemId = orderItemDto.ItemId,
                Quantity =1,
            };
            order.Result.Status = OrderStatus.InProgress.ToString();
            order.Result.TotalAmount += item.Price;
            order.Result.UpdatedAt = DateTime.Now;
            item.StockQuantity -= 1;
            await _unitOfWork.OrderItems.CreateAsync(newOrderITem);
            await _unitOfWork.SaveChangesAsync();
            return ResultResponse<OrderItemDto>.Pass(_mapper.Map<OrderItemDto>(newOrderITem),StatusCodes.Status200OK);
        }

        public async Task<ResultResponse<List<OrderDto>>> GetAllOrders()
        {
            _logger.LogInformation("all order are retrieved");
            return ResultResponse<List<OrderDto>>.Pass(_mapper.Map<List<OrderDto>>(await _unitOfWork.Orders.GetAllAsync()),StatusCodes.Status200OK);
        }

        public async Task<ResultResponse<Order>> GetOrder()
        {
            var customer = await _userService.GetCurrentUser();
            if (customer == null)
            {
                return ResultResponse<Order>.Fail("customer is not found", ErrorTypes.NotFound, StatusCodes.Status404NotFound);
            }
            var order = await _unitOfWork.OrderRepository.GetOrder(customer.Result.Id);
            if(order == null)
            {
                Console.WriteLine("order not found from get order method");
                return ResultResponse<Order>.Fail("order is not found",ErrorTypes.NotFound,StatusCodes.Status404NotFound);
            }
            return ResultResponse<Order>.Pass(order,StatusCodes.Status200OK);
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
            var orderItem = await _unitOfWork.OrderItems.GetFirstOrDefault(a => a.OrderId == order.Result.Id && a.ItemId == itemId);
            if (orderItem == null)
            {
                _logger.LogWarning("order item is not found");
                throw new ArgumentException("order item is not found");
            }
            order.Result.Status = OrderStatus.InProgress.ToString();
            order.Result.TotalAmount -= orderItem.Quantity * item.Price;
            order.Result.UpdatedAt = DateTime.Now;
            item.StockQuantity += orderItem.Quantity;
            await _unitOfWork.OrderRepository.DeleteOrderItem(order.Result.Id,itemId);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<ResultResponse<List<OrderItem>>> GetOrderItems()
        {
            var order = await GetOrder();
            if (order == null)
            {
                return ResultResponse<List<OrderItem>>.Fail("order is not found", ErrorTypes.NotFound, StatusCodes.Status404NotFound);
            }
            var orderItems = await _unitOfWork.OrderRepository.GetOrderItems(order.Result.Id);
            return ResultResponse<List<OrderItem>>.Pass(orderItems,StatusCodes.Status200OK);
        }

        public async Task CancelOrder()
        {
            var order = await GetOrder();
            //if (order == null)
            //{
            //    return ResultResponse<List<OrderItem>>.Fail("order is not found", ErrorTypes.NotFound, StatusCodes.Status404NotFound);
            //}
            order.Result.TotalAmount = 0;
            order.Result.UpdatedAt = DateTime.Now;
            order.Result.Status = OrderStatus.Cancelled.ToString();
            var orderItems=await GetOrderItems();
            await _unitOfWork.OrderRepository.DeleteOrderItems(order.Result.Id);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<ResultResponse<List<OrderItemDto>>> GetOrderItemsById(int orderId)
        {
            //var order = await _unitOfWork.OrderRepository.GetOrder(_userService.GetCurrentUser().Result.Result.Id);
            var order=await GetOrder();
            if(order == null)
            {
                Console.WriteLine("not found");
                return ResultResponse<List<OrderItemDto>>.Fail("order is not found",ErrorTypes.NotFound,StatusCodes.Status404NotFound);
            }
            var result=await _unitOfWork.OrderRepository.GetOrderItemsById(orderId);
            if (result == null)
            {
                return ResultResponse<List<OrderItemDto>>.Fail("", ErrorTypes.NotFound, StatusCodes.Status404NotFound);
            }
            return ResultResponse<List<OrderItemDto>>.Pass(_mapper.Map<List<OrderItemDto>>(result),StatusCodes.Status200OK);
        }

        public async Task<ResultResponse<OrderItemDto>> IncreaseQuantityOfItem(OrderItemDto orderItemDto)
        {
            var order = await GetOrder();
            var item = await _unitOfWork.Items.GetAsync(orderItemDto.ItemId);
            if (item == null)
            {
                _logger.LogWarning("item is not found so you cannot increase its quantity");
                return ResultResponse<OrderItemDto>.Fail("item is not found so you cannot increase its quantity", ErrorTypes.NotFound, StatusCodes.Status404NotFound);
            }
            var orderItem = await _unitOfWork.OrderItems.GetFirstOrDefault(a => a.ItemId == orderItemDto.ItemId && a.OrderId == order.Result.Id);
            if (orderItem == null)
            {
                _logger.LogWarning("order item is not found so you cannot increase its quantity");
                return ResultResponse<OrderItemDto>.Fail("order item is not found so you cannot increase its quantity", ErrorTypes.NotFound, StatusCodes.Status404NotFound);
            }
            orderItem.Quantity++;
            if (orderItem.Quantity > item.StockQuantity)
            {
                _logger.LogWarning("the stock quantity is not enough");
                return ResultResponse<OrderItemDto>.Fail("the stock quantity is not enough", ErrorTypes.BadRequest, StatusCodes.Status400BadRequest);
            }

            order.Result.Status = OrderStatus.InProgress.ToString();
            order.Result.TotalAmount += item.Price;
            order.Result.UpdatedAt = DateTime.Now;
            item.StockQuantity--;
            await _unitOfWork.SaveChangesAsync();
            return ResultResponse<OrderItemDto>.Pass(_mapper.Map<OrderItemDto>(orderItem), StatusCodes.Status200OK);
        }

        public async Task<ResultResponse<OrderItemDto>> DecreaseQuantityOfItem(OrderItemDto orderItemDto)
        {
            var order = await GetOrder();
            var item = await _unitOfWork.Items.GetAsync(orderItemDto.ItemId);
            if (item == null)
            {
                _logger.LogWarning("item is not found so you cannot decrease its quantity");
                return ResultResponse<OrderItemDto>.Fail("item is not found so you cannot decrease its quantity", ErrorTypes.NotFound, StatusCodes.Status404NotFound);
            }
            var orderItem = await _unitOfWork.OrderItems.GetFirstOrDefault(a => a.ItemId == orderItemDto.ItemId && a.OrderId == order.Result.Id);
            if (orderItem == null)
            {
                _logger.LogWarning("order item is not found so you cannot decrease its quantity");
                return ResultResponse<OrderItemDto>.Fail("order item is not found so you cannot decrease its quantity", ErrorTypes.NotFound, StatusCodes.Status404NotFound);
            }
            orderItem.Quantity--;
            if (orderItem.Quantity <1)
            {
                _logger.LogWarning("you can't get quantity less than one");
                return ResultResponse<OrderItemDto>.Fail("you can't get quantity less than one", ErrorTypes.BadRequest, StatusCodes.Status400BadRequest);
            }

            order.Result.Status = OrderStatus.InProgress.ToString();
            order.Result.TotalAmount -= item.Price;
            order.Result.UpdatedAt = DateTime.Now;
            item.StockQuantity++;
            await _unitOfWork.SaveChangesAsync();
            return ResultResponse<OrderItemDto>.Pass(_mapper.Map<OrderItemDto>(orderItem), StatusCodes.Status200OK);
        }
    }
}
