using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StoreWebApi.DTO;
using StoreWebApi.Interfaces;
using StoreWebApi.Models;

namespace StoreWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrder _orderService;

        public OrderController(IOrder orderService)
        {
            _orderService = orderService;
        }
        /// <summary>
        /// create order 
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> createOrder()
        {
            return Ok(await _orderService.createOrder());
        }
        /// <summary>
        /// get iall orders
        /// </summary>
        [HttpGet("allOrders")]
        public async Task<IActionResult> getAllOrders()
        {
            return Ok(await _orderService.getAllOrders());
        }
        /// <summary>
        /// add item to order
        /// </summary>
        [HttpPut("addItem")]
        public async Task<IActionResult> addOrderITemToOrder(int itemId, int quantity)
        {
            return Ok(await _orderService.AddOrderItemToOrder(itemId, quantity));
        }
        /// <summary>
        /// delete item from order
        /// </summary>
        [HttpPut("deleteItem")]
        public async Task<IActionResult> deleteOrderItemFromOrder(int itemId)
        {
            await _orderService.deleteOrderItemFromOrder(itemId);
            return Ok();
        }
        /// <summary>
        /// get order
        /// </summary>
        [HttpGet("order")]
        public async Task<IActionResult> getOrder()
        {
            return Ok(await _orderService.getOrder());
        }
        /// <summary>
        /// get order items
        /// </summary>
        [HttpGet("OrderItems")]
        public async Task<IActionResult> getOrderItems()
        {
            return Ok(await _orderService.getOrderItems());
        }
        /// <summary>
        /// cancel order
        /// </summary>
        [HttpPut("CancelOrder")]
        public async Task<IActionResult> cancelOrder()
        {
            await _orderService.CancelOrder();
            return Ok();
        }
    }
}
