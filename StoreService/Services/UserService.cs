using AutoMapper;
using Microsoft.IdentityModel.Tokens;
using StoreService.DTO;
using StoreDomain.Enums;
using StoreService.Interfaces;
using StoreDomain.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Logging;
using StoreService.ResponseModel;
using Microsoft.AspNetCore.Http;

namespace StoreService.Services
{
    public class UserService:IUserService
    {
        private readonly Jwt _jwt;
        private readonly IMapper _mapper;
        private readonly IUnitOfWorkServiceForStoreDb _unitOfWork;
        private readonly ILogger<UserService> _logger;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IWalletService _walletService;
        public UserService(Jwt jwt, IMapper mapper, IUnitOfWorkServiceForStoreDb unitOfWork,ILogger<UserService> logger, IHttpContextAccessor contextAccessor, IWalletService walletService)
        {
            _jwt = jwt;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _logger = logger;
            _walletService = walletService;
            _contextAccessor = contextAccessor;
        }
        public async Task<SigningResponse> SignUp(string userName, string email, string password, UserRole role)
        {
            var newUser = new User
            {
                UserName = userName,
                Email = email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
                Role = role.ToString(),
                CreatedAt = DateTime.Now,
            };
            if (newUser.Role == UserRole.Customer.ToString())
            {
                await _walletService.CreateWalletToUser(email);
                var newUserWallet=await _walletService.GetWalletOfUser(email);
                //newUser.Balance = newUserWallet.Balance;
            }
            await _unitOfWork.Users.CreateAsync(newUser);
            await _unitOfWork.SaveChangesAsync();
            return new SigningResponse
            {
                User = _mapper.Map<UserDto>(newUser),
                jwtToken = await GenerateJwtToken(newUser.Email),
                RefreshToken= _mapper.Map<RefreshTokenDto>(await CreateRefreshToken(newUser.Email))
            };
        }
        public async Task<SigningResponse> SignIn(string userEmail, string password)
        {
            var user = await GetUserByEmail(userEmail);
            if (user == null || !(BCrypt.Net.BCrypt.Verify(password, user.PasswordHash)))
            {
                _logger.LogWarning("your email or password is not found");
                throw new ArgumentException("user is not found");
            }
            return new SigningResponse
            {
                User = _mapper.Map<UserDto>(user),
                jwtToken = await GenerateJwtToken(user.Email),
                RefreshToken= _mapper.Map<RefreshTokenDto>(await CreateRefreshToken(user.Email))
            };
        }
        public async Task<User> GetUserByEmail(string email)
        {
            var user = await _unitOfWork.Users.GetFirstOrDefault(a=>a.Email==email);
            if (user == null)
            {
                _logger.LogWarning("user is not found with this email");
                throw new ArgumentException("user is not found");
            }
            return user;
        }
        public async Task<string> GenerateJwtToken(string userEmail)
        {
            var user = await GetUserByEmail(userEmail);
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Issuer = _jwt.Issuer,
                Audience = _jwt.Audience,
                Expires = DateTime.Now.AddMinutes(2),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Signingkey)),
                SecurityAlgorithms.HmacSha256Signature),
                Subject = new System.Security.Claims.ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Email,user.Email),
                    new Claim(ClaimTypes.Role,user.Role),
                })
            };
            var securityToken = tokenHandler.CreateToken(tokenDescriptor);
            var accessToken = tokenHandler.WriteToken(securityToken);
            return accessToken;
        }
        public string GenerateRandomRefreshToken()
        {
            return Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        }

        public async Task<RefreshToken> CreateRefreshToken(string userEmail)
        {
            var user=await GetUserByEmail(userEmail);
            var newRefreshToken = new RefreshToken
            {
                User=user,
                UserId=user.Id,
                Token=GenerateRandomRefreshToken(),
                CreatedAt=DateTime.Now,
                ExpiredAt=DateTime.Now.AddMinutes(3),
            };
            await _unitOfWork.RefreshTokens.CreateAsync(newRefreshToken);
            await _unitOfWork.SaveChangesAsync();
            return newRefreshToken;
        }
        public async Task<SigningResponse> RefreshToken(string userEmail)
        {
            var user=await GetUserByEmail(userEmail);
            var refreshToken = await _unitOfWork.UserRepository.GetLastRefreshToken(user.Id);
            if(refreshToken==null || !refreshToken.isValid)
            {
                _logger.LogInformation("your refresh token is expired");
                //throw new ArgumentException("your refresh token is expired");
            }
            refreshToken.Token=GenerateRandomRefreshToken();
            refreshToken.CreatedAt=DateTime.Now;
            refreshToken.ExpiredAt=DateTime.Now.AddMinutes(3);
            await _unitOfWork.SaveChangesAsync();
            return new SigningResponse
            {
                User=_mapper.Map<UserDto>(user),
                jwtToken= await GenerateJwtToken(userEmail),
                RefreshToken=_mapper.Map<RefreshTokenDto>(refreshToken),    
            };
        }
        public async Task<User> GetCurrentUser()
        {
            var currentUserEmail = _contextAccessor.HttpContext?.User.FindFirst(ClaimTypes.Email)?.Value;
            if (currentUserEmail == null)
            {
                throw new ArgumentException("user is not found");
            }
            return await GetUserByEmail(currentUserEmail);
        }
        public async Task SignOut()
        {
            var user=await GetCurrentUser();
            var refreshToken = await _unitOfWork.UserRepository.GetLastRefreshToken(user.Id);
            if(refreshToken == null)
            {
                throw new ArgumentException("your token is not found");
            }
            refreshToken.ExpiredAt = DateTime.Now;
            await _unitOfWork.SaveChangesAsync();
        }
        
    }
}
