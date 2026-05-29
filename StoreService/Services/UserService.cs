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
        public async Task<ResultResponse<SigningResponse>> SignUp(string userName, string email, string password, UserRole role)
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
            return ResultResponse<SigningResponse>.Pass(new SigningResponse
            {
                User = _mapper.Map<UserDto>(newUser),
                jwtToken = await GenerateJwtToken(newUser.Email),
                RefreshToken= _mapper.Map<RefreshTokenDto>(await CreateRefreshToken(newUser.Email))
            },StatusCodes.Status201Created);
        }
        public async Task<ResultResponse<SigningResponse>> SignIn(string userEmail, string password)
        {
            var user = await _unitOfWork.Users.GetFirstOrDefault(a=>a.Email==userEmail);
            if (user == null || !(BCrypt.Net.BCrypt.Verify(password, user.PasswordHash)))
            {
                _logger.LogWarning("your email or password is not correct");
                return ResultResponse<SigningResponse>.Fail("your email or password is not correct",ErrorTypes.BadRequest,StatusCodes.Status400BadRequest);
            }
            return ResultResponse<SigningResponse>.Pass(new SigningResponse
            {
                User = _mapper.Map<UserDto>(user),
                jwtToken = await GenerateJwtToken(user.Email),
                RefreshToken= _mapper.Map<RefreshTokenDto>(await CreateRefreshToken(user.Email))
            },StatusCodes.Status200OK);
        }
        public async Task<ResultResponse<User>> GetUserByEmail(string email)
        {
            var user = await _unitOfWork.Users.GetFirstOrDefault(a=>a.Email==email);
            if (user == null)
            {
                _logger.LogWarning("user is not found with this email");
                return ResultResponse<User>.Fail("user is not found with this email", ErrorTypes.NotFound, StatusCodes.Status404NotFound);

            }
            return ResultResponse<User>.Pass(user,StatusCodes.Status200OK);
        }
        public async Task<string> GenerateJwtToken(string userEmail)
        {
            var user = await _unitOfWork.Users.GetFirstOrDefault(a => a.Email == userEmail);
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Issuer = _jwt.Issuer,
                Audience = _jwt.Audience,
                Expires = DateTime.Now.AddMinutes(30),
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
            var user=await _unitOfWork.Users.GetFirstOrDefault(a => a.Email == userEmail);
            var newRefreshToken = new RefreshToken
            {
                User=user,
                UserId=user.Id,
                Token=GenerateRandomRefreshToken(),
                CreatedAt=DateTime.Now,
                ExpiredAt=DateTime.Now.AddMinutes(50),
            };
            await _unitOfWork.RefreshTokens.CreateAsync(newRefreshToken);
            await _unitOfWork.SaveChangesAsync();
            return newRefreshToken;
        }
        public async Task<ResultResponse<SigningResponse>> RefreshToken(string userEmail)
        {
            var user=await _unitOfWork.Users.GetFirstOrDefault(a=>a.Email==userEmail);
            if (user == null)
            {
                return ResultResponse<SigningResponse>.Fail("user is not found with this email", ErrorTypes.NotFound, StatusCodes.Status404NotFound);
            }
            var refreshToken = await _unitOfWork.UserRepository.GetLastRefreshToken(user.Id);
            if(refreshToken == null)
            {
                _logger.LogInformation("your refresh token is expired");
                return ResultResponse<SigningResponse>.Fail("refresh token is not found", ErrorTypes.NotFound, StatusCodes.Status404NotFound);
            }
            refreshToken.Token=GenerateRandomRefreshToken();
            refreshToken.CreatedAt=DateTime.Now;
            refreshToken.ExpiredAt=DateTime.Now.AddMinutes(50);
            await _unitOfWork.SaveChangesAsync();
            return ResultResponse<SigningResponse>.Pass(new SigningResponse
            {
                User=_mapper.Map<UserDto>(user),
                jwtToken= await GenerateJwtToken(userEmail),
                RefreshToken=_mapper.Map<RefreshTokenDto>(refreshToken),    
            },StatusCodes.Status200OK);
        }
        public async Task<ResultResponse<User>> GetCurrentUser()
        {
            var currentUserEmail = _contextAccessor.HttpContext?.User.FindFirst(ClaimTypes.Email)?.Value;
            if (currentUserEmail == null)
            {
                return ResultResponse<User>.Fail("user is not found",ErrorTypes.NotFound,StatusCodes.Status404NotFound);
            }
            return await GetUserByEmail(currentUserEmail);
        }
        public async Task SignOut()
        {
            var user=await GetCurrentUser();
            var refreshToken = await _unitOfWork.UserRepository.GetLastRefreshToken(user.Result.Id);
            if(refreshToken == null)
            {
                throw new ArgumentException("your token is not found");
            }
            refreshToken.ExpiredAt = DateTime.Now;
            await _unitOfWork.SaveChangesAsync();
        }
        
    }
}
