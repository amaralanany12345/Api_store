using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StoreService.DTO;
using StoreWebApi.ExceptionHandler;
using StoreService.Interfaces;
using StoreDomain.Models;

namespace StoreWebApi.Controllers
{
    [Route("api/items")]
    [ApiController]
    [Authorize(Policy = "refreshTokenIsValid")]
    public class ItemController : ControllerBase
    {
        private readonly IItemService _ItemService;
        
        public ItemController(IItemService itemService)
        {
            _ItemService = itemService;
        }
        /// <summary>
        /// create item
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateItem([FromBody]ItemDto itemData)
        {
            return Ok(await _ItemService.CreateItem(itemData.Name,itemData.Price,itemData.StockQuantity,itemData.CategoryName));
        }
        /// <summary>
        /// get all items
        /// </summary>
        [HttpGet]
        [Authorize(Roles = "Admin,Customer")]
        public async Task<IActionResult> GetAllItems()
        {
            return Ok(await _ItemService.GetAllItems());
        }
        /// <summary>
        /// get item by name
        /// </summary>
        [HttpGet("{ITemId}")]
        [Authorize(Roles = "Admin,Customer")]
        public async Task<IActionResult> GetItem(int ITemId)
        {
            return Ok(await _ItemService.GetITem(ITemId));
        }
        
        /// <summary>
        /// get items by category name
        /// </summary>
        
        [HttpGet("category/{categoryName}")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> GetItemsByCategory(int ITemId, int pageSize, int pageNumber)
        {
            return Ok(await _ItemService.GetITemByCategory(ITemId, pageSize,pageNumber));
        }

        /// <summary>
        /// delete item by item name
        /// </summary>
        
        [HttpDelete("{itemId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteItem(int itemId)
        {
            await _ItemService.DeleteItem(itemId);
            return Ok();
        }

        /// <summary>
        /// update item
        /// </summary>
        
        [HttpPut("{itemName}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateItem(int itemId, string newName, int newPrice, int stockQuantity)
        {
            return Ok(await _ItemService.UpdateItem(itemId, newName, newPrice, stockQuantity));
        }


    }
}
