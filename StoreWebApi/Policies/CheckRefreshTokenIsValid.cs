using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using StoreWebApi.Models;
using StoreWebApi.zAppContexts;
using System.Security.Claims;

namespace StoreWebApi.bolicesis
{
    public class CheckRefreshTokenIsValid : AuthorizationHandler<ValidRefreshToken>
    {
        private readonly AppDbContext _context;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly ILogger<CheckRefreshTokenIsValid> _logger;
        public CheckRefreshTokenIsValid(AppDbContext context, IHttpContextAccessor contextAccessor, ILogger<CheckRefreshTokenIsValid> logger)
        {
            _context = context;
            _contextAccessor = contextAccessor;
            _logger = logger;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, ValidRefreshToken requirement)
        {
            var currentUserEmail = _contextAccessor.HttpContext?.User.FindFirst(ClaimTypes.Email)?.Value;
            if (currentUserEmail == null)
            {
                _logger.LogWarning("user email is not found");
                throw new ArgumentException("user email is not found");
            }
            var user = await _context.Users.Where(a => a.Email == currentUserEmail).FirstOrDefaultAsync();
            if (user == null)
            {
                _logger.LogWarning("user is not found");
                throw new ArgumentException("user is not found");
            }
            var currentUserRefreshToken = await _context.RefreshTokens.Where(a => a.UserId == user.Id).OrderByDescending(A => A.CreatedAt).FirstOrDefaultAsync();
            if (currentUserRefreshToken == null)
            {
                _logger.LogWarning("your token is not found");
                throw new ArgumentException("your token is not found");
            }
            if (currentUserRefreshToken.isValid)
            {
                context.Succeed(requirement);
            }
            else
            {
                throw new ArgumentException("your token is expired");
            }
        }
    }
}
