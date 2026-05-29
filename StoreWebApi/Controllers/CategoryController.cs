using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StoreWebApi.Actions;
using StoreService.DTO;
using StoreService.Interfaces;
using StoreDomain.Models;

namespace StoreWebApi.Controllers
{
    [Route("api/categories")]
    [ApiController]
    //[Authorize(Policy = "refreshTokenIsValid")]
    [ServiceFilter(typeof(ValidateRefreshTokenAttribute))]

    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;
        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }
        /// <summary>
        /// create category
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateCategory([FromBody] CategoryDto categoryData)
        {
            var result=await _categoryService.CreateCategory(categoryData.Name,categoryData.Description);
            if (!result.Success)
            {
                return StatusCode(result.StatusCode, result.Error);
            }
            return StatusCode(result.StatusCode, result.Result);
        }
        /// <summary>
        /// get all categories
        /// </summary>
        [HttpGet]
        [Authorize(Roles = "Admin,Customer")]
        public async Task<IActionResult> GetAllCategories()
        {
            var result=await _categoryService.GetAllCategories();
            if (!result.Success)
            {
                return StatusCode(result.StatusCode, result.Error);
            }
            return StatusCode(result.StatusCode,result.Result);
        }
        /// <summary>
        /// get category by name
        /// </summary>
        [HttpGet("{categoryId}")]
        [Authorize(Roles = "Admin,Customer")]
        public async Task<IActionResult> GetCategory(int categoryId)
        {
            var result=await _categoryService.GetCategory(categoryId);
            if (!result.Success)
            {
                return StatusCode(result.StatusCode, result.Error);
            }
            return StatusCode(result.StatusCode, result.Result);
        }
        /// <summary>
        /// delete category by name
        /// </summary>
        [HttpDelete("{CategoryId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteCategory(int CategoryId)
        {
            await _categoryService.DeleteCategory(CategoryId);      
            return Ok();
        }
        /// <summary>
        /// update category
        /// </summary>
        [HttpPut("{CategoryId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateCategory(int CategoryId, string newName, string newDescription)
        {
            var result = await _categoryService.UpdateCategory(CategoryId, newName, newDescription);
            if (!result.Success)
            {
                return StatusCode(result.StatusCode, result.Error);
            }
            return StatusCode(result.StatusCode, result.Result);
        }

    }
}
