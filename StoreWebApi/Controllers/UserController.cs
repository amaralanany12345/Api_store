using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StoreWebApi.DTO;
using StoreWebApi.Enums;
using StoreWebApi.Interfaces;
using StoreWebApi.Models;

namespace StoreWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUser _userService;

        public UserController(IUser userService)
        {
            _userService = userService;
        }
        /// <summary>
        /// user register 
        /// </summary>
        [HttpPost("signUp")]
        public async Task<IActionResult> SignUp(string userName, string email, string password, UserRole role, int? balance)
        {
            return Ok(await _userService.signUp(userName,email,password,role,balance));
        }
        /// <summary>
        /// sign in 
        /// </summary>
        [HttpPost("signIn")]
        public async Task<IActionResult> SignIn(string email,string password)
        {
            return Ok(await _userService.signIn(email, password));
        }
        /// <summary>
        /// refresh the token
        /// </summary>
        [HttpPut("refreshToken")]
        public async Task<IActionResult> refreshToken(string userEmail)
        {
            return Ok(await _userService.refreshToken(userEmail));
        }
        /// <summary>
        /// get the current user using the httpContext
        /// </summary>
        [HttpGet("currentUser")]
        public async Task<IActionResult> getCurrentUser()
        {
            return Ok(await _userService.getCurrentUser());
        }
        /// <summary>
        /// Sign out
        /// </summary>
        [HttpPut("signOut")]
        public async Task<IActionResult> signOut()
        {
            await _userService.signOut();
            return Ok();
        }




    }
}
