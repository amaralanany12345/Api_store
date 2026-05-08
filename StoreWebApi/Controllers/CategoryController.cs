using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StoreWebApi.DTO;
using StoreWebApi.Interfaces;
using StoreWebApi.Services;

namespace StoreWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategory _categoryService;
        public CategoryController(ICategory categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpPost]
        public async Task<ActionResult<CategoryDto>> createCategory(string name, string descrption)
        {
            return Ok(await _categoryService.createCategory(name, descrption));
        }
        [HttpGet("AllCategories")]
        [Authorize(Roles = "Admin,Customer")]
        //[Authorize(Roles = "Admin")]
        public async Task<ActionResult<List<CategoryDto>>> getAllCategories()
        {
            return Ok(await _categoryService.getAllCategories());
        }
        [HttpGet]
        public async Task<ActionResult<CategoryDto>> getCategoryByName(string CategoryName)
        {
            return Ok(await _categoryService.getCategory(CategoryName));
        }
        [HttpDelete]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> deleteCategory(int CategoryId)
        {
            await _categoryService.deleteCategory(CategoryId);
            return Ok();
        }
        [HttpPut]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<CategoryDto>> updateCategory(int CategoryId, string newName, string newDescription)
        {
            return Ok(await _categoryService.updateCategory(CategoryId, newName, newDescription));
        }
    }
}
