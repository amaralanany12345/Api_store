using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StoreWebApi.DTO;
using StoreWebApi.Interfaces;
using StoreWebApi.Models;

namespace StoreWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ItemController : ControllerBase
    {
        private readonly IItem _ItemService;
        
        public ItemController(IItem itemService)
        {
            _ItemService = itemService;
        }
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ItemDto>> createItem(string name, int price, int stockQuantity, int categoryId)
        {
            return Ok(await _ItemService.createItem(name, price, stockQuantity, categoryId));
        }
        [HttpGet("allItems")]
        [Authorize(Roles = "Admin,Customer")]
        public async Task<ActionResult<List<ItemDto>>> getAllItems()
        {
            return Ok(await _ItemService.getAllItems());
        }
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ItemDto>> getItemByName(string itemName)
        {
            return Ok(await _ItemService.getITem(itemName));
        }
        [HttpGet("byCategory")]
        [Authorize(Roles = "Admin,Customer")]
        public async Task<ActionResult<List<ItemDto>>> getItemsByCategory(string categoryName)
        {
            return Ok(await _ItemService.getITemByCategoryName(categoryName));
        }
        [HttpDelete]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> deleteItem(int itemId)
        {
            await _ItemService.deleteItem(itemId);
            return Ok();
        }
        [HttpPut]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ItemDto>> updateItem(int ItemId, string newName, int newPrice, int stockQuantity)
        {
            return Ok(await _ItemService.updateItem(ItemId, newName, newPrice, stockQuantity));
        }


    }
}
