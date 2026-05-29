using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StoreService.DTO;
using StoreDomain.Enums;
using StoreService.Interfaces;
using StoreDomain.Models;
using StoreService.RequestModels;
using Microsoft.AspNetCore.Http.HttpResults;

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
            var result=await _userService.SignUp(registerRequest.userName, registerRequest.Email, registerRequest.Password, registerRequest.Role);
            if (!result.Success)
            {
                return StatusCode(result.StatusCode, result.Error);
            }
            return StatusCode(result.StatusCode, result.Result);
        }
        /// <summary>
        /// sign in 
        /// </summary>
        [HttpPost("login")]
        public async Task<IActionResult> SignIn([FromBody] LoginRequest userRequest)
        {
            var result=await _userService.SignIn(userRequest.Email, userRequest.Password);
            if (!result.Success)
            {
                return StatusCode(result.StatusCode, result.Error);
            }
            return StatusCode(result.StatusCode, result.Result);
        }
        /// <summary>
        /// refresh the token
        /// </summary>
        [HttpPut("refresh-token")]
        public async Task<IActionResult> RefreshToken(string userEmail)
        {
            var result=await _userService.RefreshToken(userEmail);
            if (!result.Success)
            {
                return StatusCode(result.StatusCode, result.Error);
            }
            return StatusCode(result.StatusCode, result.Result);
        }
        /// <summary>
        /// get the current user using the httpContext
        /// </summary>
        [HttpGet("currentUser")]
        public async Task<IActionResult> GetCurrentUser()
        {
            var result=await _userService.GetCurrentUser();
            if (!result.Success)
            {
                return StatusCode(result.StatusCode, result.Error);
            }
            return StatusCode(result.StatusCode, result.Result);
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
