using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using StoreWebApi.zAppContexts;
using System.Security.Claims;

namespace StoreWebApi.Actions
{
    public class ValidateRefreshTokenAttribute:ActionFilterAttribute
    {
        private readonly AppDbContext _context;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly ILogger<ValidateRefreshTokenAttribute> _logger;

        public ValidateRefreshTokenAttribute(AppDbContext context, IHttpContextAccessor contextAccessor, ILogger<ValidateRefreshTokenAttribute> logger)
        {
            _context = context;
            _contextAccessor = contextAccessor;
            _logger = logger;
        }

        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
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
                await next();
            }
            else
            {
                _logger.LogWarning("your token is expired an not valid please refresh you token");
                context.Result = new UnauthorizedObjectResult("your token is expired");
                return;
            }
        }
    }
}
