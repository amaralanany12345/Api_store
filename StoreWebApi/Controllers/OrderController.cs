using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StoreService.DTO;
using StoreService.Interfaces;
using StoreDomain.Models;

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
            return Ok(await _orderService.CreateOrder());
        }
        /// <summary>
        /// get all orders
        /// </summary>
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllOrders()
        {
            return Ok(await _orderService.GetAllOrders());
        }
        /// <summary>
        /// add item to order
        /// </summary>
        [HttpPost("orderItems/{itemId}")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> AddOrderITemToOrder(int itemId, int quantity)
        {
            return Ok(await _orderService.AddOrderItemToOrder(itemId, quantity));
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
        [Authorize(Roles ="Customer")]
        public async Task<IActionResult> CancelOrder()
        {
            await _orderService.CancelOrder();
            return Ok();
        }
        /// <summary>
        /// get the order Items
        /// </summary>
        [HttpGet("orderItems/{orderId}")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> GetOrderItemsById(int orderId)
        {
            return Ok(await _orderService.GetOrderItemsById(orderId));
        }

    }
}
