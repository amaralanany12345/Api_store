using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StoreService.DTO;
using StoreDomain.Enums;
using StoreService.Interfaces;
using StoreDomain.Models;
using StoreService.RequestModels;

namespace StoreWebApi.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }
        /// <summary>
        /// user register 
        /// </summary>
        [HttpPost("register")]
        public async Task<IActionResult> SignUp([FromBody] RegisterRequest registerRequest)
        {
            return Ok(await _userService.SignUp(registerRequest.userName, registerRequest.Email, registerRequest.Password, registerRequest.Role));
        }
        /// <summary>
        /// sign in 
        /// </summary>
        [HttpPost("login")]
        public async Task<IActionResult> SignIn([FromBody] LoginRequest userRequest)
        {
            return Ok(await _userService.SignIn(userRequest.Email, userRequest.Password));
        }
        /// <summary>
        /// refresh the token
        /// </summary>
        [HttpPut("refresh-token")]
        public async Task<IActionResult> RefreshToken(string userEmail)
        {
            return Ok(await _userService.RefreshToken(userEmail));
        }
        /// <summary>
        /// get the current user using the httpContext
        /// </summary>
        [HttpGet("currentUser")]
        public async Task<IActionResult> GetCurrentUser()
        {
            return Ok(await _userService.GetCurrentUser());
        }
        /// <summary>
        /// Sign out
        /// </summary>
        [HttpPut("logout")]
        public async Task<IActionResult> SignOut()
        {
            await _userService.SignOut();
            return Ok();
        }




    }
}
