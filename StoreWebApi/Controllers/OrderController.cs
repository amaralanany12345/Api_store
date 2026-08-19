using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StoreService.DTO;
using StoreService.Interfaces;
using StoreDomain.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using System.Net.WebSockets;
using MailKit.Search;
using StoreService.ResponseModel;

namespace StoreWebApi.Controllers
{
    [Route("api/orders")]
    [ApiController]
    //[Authorize(Policy = "refreshTokenIsValid")]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }
        /// <summary>
        /// create order 
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> CreateOrder()
        {
            var result=await _orderService.CreateOrder();
            if (!result.Success)
            {
                return StatusCode(result.StatusCode, result.Error);
            }
            return StatusCode(result.StatusCode, result.Result);
        }
        /// <summary>
        /// get all orders
        /// </summary>
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllOrders()
        {
            var result=await _orderService.GetAllOrders();
            if (!result.Success)
            {
                return StatusCode(result.StatusCode, result.Error);
            }
            return StatusCode(result.StatusCode, result.Result);
        }
        /// <summary>
        /// add item to order
        /// </summary>
        [HttpPost("orderItems")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> AddOrderITemToOrder([FromBody] OrderItemDto orderItemDto)
        {
            var result=await _orderService.AddOrderItemToOrder(orderItemDto);
            if (!result.Success)
            {
                return StatusCode(result.StatusCode, result.Error);
            }
            return StatusCode(result.StatusCode, result.Result);
        }
        [HttpPut("orderItems/Increase")]
        public async Task<IActionResult> IncreaseQuantityOfItem(OrderItemDto orderItemDto)
        {
            var result = await _orderService.IncreaseQuantityOfItem(orderItemDto);
            if (!result.Success)
            {
                return StatusCode(result.StatusCode, result.Error);
            }
            return StatusCode(result.StatusCode, result.Result);
        }
        [HttpPut("orderItems/Decrease")]
        public async Task<IActionResult> DecreaseQuantityOfItem(OrderItemDto orderItemDto)
        {
            var result = await _orderService.DecreaseQuantityOfItem(orderItemDto);
            if (!result.Success)
            {
                return StatusCode(result.StatusCode, result.Error);
            }
            return StatusCode(result.StatusCode, result.Result);
        }

        /// <summary>
        /// delete item from order
        /// </summary>
        [HttpDelete("orderItems/{itemId}")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> DeleteOrderItemFromOrder(int itemId)
        {
            await _orderService.DeleteOrderItemFromOrder(itemId);
            return Ok();
        }
        /// <summary>
        /// cancel order
        /// </summary>
        [HttpPut("cancel")]
        [Authorize(Roles = "Admin,Customer")]
        public async Task<IActionResult> CancelOrder()
        {
            await _orderService.CancelOrder();
            return Ok();
        }
        /// <summary>
        /// get the order Items
        /// </summary>
        [HttpGet("orderItems/{orderId}")]
        [Authorize(Roles = "Admin,Customer")]
        public async Task<IActionResult> GetOrderItemsById(int orderId)
        {
            var result=await _orderService.GetOrderItemsById(orderId);
            if (!result.Success)
            {
                return StatusCode(result.StatusCode, result.Error);
            }
            return StatusCode(result.StatusCode, result.Result);
        }
        [HttpGet("current")]
        public async Task<IActionResult> GetOrder()
        {
            var result = await _orderService.GetOrder();
            if (!result.Success)
            {
                return StatusCode(result.StatusCode, result.Error);
            }
            return StatusCode(result.StatusCode, result.Result);
        }
        [HttpGet("orderItems")]
        public async Task<IActionResult> GetOrderItems()
        {
            var result = await _orderService.GetOrderItems();
            if (!result.Success)
            {
                return StatusCode(result.StatusCode, result.Error);
            }
            return StatusCode(result.StatusCode, result.Result);
        }

    }
}
