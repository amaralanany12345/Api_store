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
        private readonly IMapper _mapper;

        public UserController(IUser userService, IMapper mapper)
        {
            _userService = userService;
            _mapper = mapper;
        }
        [HttpPost("signUp")]
        public async Task<ActionResult<SigningResponse>> SignUp(string userName, string email, string password, UserRole role, int? balance)
        {
            return Ok(await _userService.signUp(userName,email,password,role,balance));
        }
        [HttpPost("signIn")]
        public async Task<ActionResult<SigningResponse>> SignIn(string email, string password)
        {
            return Ok(await _userService.signIn(email, password));
        }
        [HttpPut("refreshToken")]
        public async Task<ActionResult<SigningResponse>> refreshToken(int userId)
        {
            return Ok(await _userService.refreshToken(userId));
        }

    }
}
