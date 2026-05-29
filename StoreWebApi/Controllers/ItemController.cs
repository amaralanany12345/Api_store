using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StoreService.DTO;
using StoreWebApi.ExceptionHandler;
using StoreService.Interfaces;
using StoreDomain.Models;
using StoreWebApi.Actions;

namespace StoreWebApi.Controllers
{
    [Route("api/items")]
    [ApiController]
    //[Authorize(Policy = "refreshTokenIsValid")]
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
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateItem([FromBody]ItemDto itemData)
        {
            var result=await _ItemService.CreateItem(itemData.Name,itemData.Price,itemData.StockQuantity,itemData.CategoryName);
            if (!result.Success)
            {
                return StatusCode(result.StatusCode, result.Error);
            }
            return StatusCode(result.StatusCode, result.Result);
        }
        /// <summary>
        /// get all items
        /// </summary>
        [HttpGet]
        //[Authorize(Roles = "Admin,Customer")]
        public async Task<IActionResult> GetAllItems()
        {
            var result= await _ItemService.GetAllItems();
            if (!result.Success)
            {
                return StatusCode(result.StatusCode, result.Error);
            }
            return StatusCode(result.StatusCode, result.Result);
        }
        /// <summary>
        /// get item by name
        /// </summary>
        [HttpGet("{ITemId}")]
        //[Authorize(Roles = "Admin,Customer")]
        public async Task<IActionResult> GetItem(int ITemId)
        {
            var result=await _ItemService.GetITem(ITemId);
            if (!result.Success)
            {
                return StatusCode(result.StatusCode, result.Error);
            }
            return StatusCode(result.StatusCode,result.Result);
        }
        
        /// <summary>
        /// get items by category name
        /// </summary>
        
        [HttpGet("category/{categoryId}")]
        //[Authorize(Roles = "Customer")]
        public async Task<IActionResult> GetItemsByCategory(int categoryId, int pageSize, int pageNumber)
        {
            var result=await _ItemService.GetITemByCategory(categoryId, pageSize,pageNumber);
            if(!result.Success)
            {
                return StatusCode(result.StatusCode,result.Error);
            }
            return StatusCode(result.StatusCode,result.Result);
        }

        /// <summary>
        /// delete item by item name
        /// </summary>
        
        [HttpDelete("{itemId}")]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteItem(int itemId)
        {
            await _ItemService.DeleteItem(itemId);
            return Ok();
        }

        /// <summary>
        /// update item
        /// </summary>
        
        [HttpPut("{itemId}")]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateItem(int itemId, string newName, int newPrice, int stockQuantity)
        {
            var result=await _ItemService.UpdateItem(itemId, newName, newPrice, stockQuantity);
            if (!result.Success)
            {
                return StatusCode(result.StatusCode, result.Error);
            }
            return StatusCode(result.StatusCode,result.Result);
        }


    }
}
