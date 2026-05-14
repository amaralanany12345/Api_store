using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StoreWebApi.DTO;
using StoreWebApi.ExceptionHandler;
using StoreWebApi.Interfaces;
using StoreWebApi.Models;

namespace StoreWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "refreshTokenIsValid")]
    public class ItemController : ControllerBase
    {
        private readonly IItem _ItemService;
        
        public ItemController(IItem itemService)
        {
            _ItemService = itemService;
        }
        /// <summary>
        /// create item
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> createItem([FromQuery]ItemDto itemData)
        {
            return Ok(await _ItemService.createItem(itemData.Name,itemData.Price,itemData.StockQuantity,itemData.CategoryName));
        }
        /// <summary>
        /// get all items
        /// </summary>
        [HttpGet("allItems")]
        [Authorize(Roles = "Admin,Customer")]
        public async Task<IActionResult> getAllItems()
        {
            return Ok(await _ItemService.getAllItems());
        }
        /// <summary>
        /// get item by name
        /// </summary>
        [HttpGet("{itemName}")]
        [Authorize(Roles = "Admin,Customer")]
        public async Task<IActionResult> getItemByName(string itemName)
        {
            return Ok(await _ItemService.getITem(itemName));
        }
        
        /// <summary>
        /// get items by category name
        /// </summary>
        
        [HttpGet("byCategory")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> getItemsByCategory(string categoryName, int pageSize, int pageNumber)
        {
            return Ok(await _ItemService.getITemByCategoryName(categoryName,pageSize,pageNumber));
        }

        /// <summary>
        /// delete item by item name
        /// </summary>
        
        [HttpDelete]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> deleteItem(string itemName)
        {
            await _ItemService.deleteItem(itemName);
            return Ok();
        }

        /// <summary>
        /// update item
        /// </summary>
        
        [HttpPut]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> updateItem(string itemName, string newName, int newPrice, int stockQuantity)
        {
            return Ok(await _ItemService.updateItem(itemName, newName, newPrice, stockQuantity));
        }


    }
}
