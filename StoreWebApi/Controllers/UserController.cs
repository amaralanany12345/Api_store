using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StoreWebApi.DTO;
using StoreWebApi.Enums;
using StoreWebApi.Interfaces;
using StoreWebApi.Models;

namespace StoreWebApi.Controllers
{
    [Route("api/auth")]
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
        [HttpPost("register")]
        public async Task<IActionResult> SignUp([FromBody] RegisterRequest registerRequest)
        {
            return Ok(await _userService.signUp(registerRequest.userName, registerRequest.Email, registerRequest.Password, registerRequest.Role));
        }
        /// <summary>
        /// sign in 
        /// </summary>
        [HttpPost("login")]
        public async Task<IActionResult> SignIn([FromBody] SignInRequest userRequest)
        {
            return Ok(await _userService.signIn(userRequest.Email, userRequest.Password));
        }
        /// <summary>
        /// refresh the token
        /// </summary>
        [HttpPut("refresh-token")]
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
        [HttpPut("logout")]
        public async Task<IActionResult> signOut()
        {
            await _userService.signOut();
            return Ok();
        }




    }
}
