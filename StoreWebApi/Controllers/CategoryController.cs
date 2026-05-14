using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StoreWebApi.DTO;
using StoreWebApi.Interfaces;
using StoreWebApi.Models;
using StoreWebApi.Services;

namespace StoreWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "refreshTokenIsValid")]
    public class CategoryController : ControllerBase
    {
        private readonly ICategory _categoryService;
        public CategoryController(ICategory categoryService)
        {
            _categoryService = categoryService;
        }
        /// <summary>
        /// create category
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> createCategory([FromQuery] CategoryDto categoryData)
        {
            return Ok(await _categoryService.createCategory(categoryData.Name,categoryData.Description));
        }
        /// <summary>
        /// get all categories
        /// </summary>
        [HttpGet("AllCategories")]
        [Authorize(Roles = "Admin,Customer")]
        public async Task<IActionResult> getAllCategories()
        {
            return Ok(await _categoryService.getAllCategories());
        }
        /// <summary>
        /// get category by name
        /// </summary>
        [HttpGet]
        [Authorize(Roles = "Admin,Customer")]
        public async Task<IActionResult> getCategoryByName(string CategoryName)
        {
            return Ok(await _categoryService.getCategory(CategoryName));
        }
        /// <summary>
        /// delete category by name
        /// </summary>
        [HttpDelete]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> deleteCategory(string CategoryName)
        {
            await _categoryService.deleteCategory(CategoryName);
            return Ok();
        }
        /// <summary>
        /// update category
        /// </summary>
        [HttpPut]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> updateCategory(string CategoryName, string newName, string newDescription)
        {
            return Ok(await _categoryService.updateCategory(CategoryName, newName, newDescription));
        }

    }
}
