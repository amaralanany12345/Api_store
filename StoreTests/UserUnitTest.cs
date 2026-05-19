using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using StoreWebApi.DTO;
using StoreWebApi.Enums;
using StoreWebApi.Interfaces;
using StoreWebApi.Models;
using StoreWebApi.Services;
using StoreWebApi.zAppContexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace StoreTests
{
    public class UserServiceTest
    {
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ILogger<UserService>> _loggerMock;
        private readonly UserService _userService;
        private readonly AppDbContext _appDbContext;
        private readonly WalletAppDbContext _walletAppDbContext;
        private readonly Mock<IUnitOfWork> _unitOfWork;
        private readonly Mock<IGenericRepo<User>> _genericRepoService;
        private readonly Mock<IWallet> _walletMockService;
        private readonly Mock<IHttpContextAccessor> _contextAccessorMock;
        private readonly Jwt _jwt;
        public UserServiceTest()
        {
            var appDbContextOptions=new DbContextOptionsBuilder<AppDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
            var WalletAppDbContextOptions=new DbContextOptionsBuilder<WalletAppDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
            _appDbContext=new AppDbContext(appDbContextOptions);
            _walletAppDbContext=new WalletAppDbContext(WalletAppDbContextOptions);
            _jwt = new Jwt
            {
                Issuer = "http://localhost:5129",
                Audience = "http://localhost:5129",
                Signingkey = "GfjkoipsfgWQEY1234fdRfg45LOPhsFFF"
            };
            _mapperMock = new Mock<IMapper>();
            _loggerMock = new Mock<ILogger<UserService>>();
            _unitOfWork = new Mock<IUnitOfWork>();
            _genericRepoService = new Mock<IGenericRepo<User>>();
            _walletMockService=new Mock<IWallet>();
            _contextAccessorMock = new Mock<IHttpContextAccessor>();
            _userService = new UserService(_appDbContext, _jwt,_mapperMock.Object, _unitOfWork.Object,
                _genericRepoService.Object,_loggerMock.Object,_contextAccessorMock.Object ,_walletMockService.Object);
        }
        [Fact]
        public async Task SignUp_ByRegisterRequest_ReturnSigningResponse()
        {
            var newUser = new User
            {
                Id=1,
                UserName="ammar",
                Email="ammar@gmail.com",
                Role=UserRole.Admin.ToString(),
                CreatedAt=DateTime.Now,
                PasswordHash=BCrypt.Net.BCrypt.HashPassword("ammar123")
            };
            var newRefreshToken = new RefreshToken
            {
                Id=1,
                User=newUser,
                UserId=newUser.Id,
                Token =Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
                CreatedAt =DateTime.Now,
                ExpiredAt=DateTime.Now.AddSeconds(30),
            };
            await _appDbContext.Users.AddAsync(newUser);
            await _appDbContext.RefreshTokens.AddAsync(newRefreshToken);
            await _appDbContext.SaveChangesAsync();

            var newUserDto = new UserDto
            {
                UserName=newUser.UserName,
                Email=newUser.Email,
                Role=UserRole.Admin.ToString(),
                CreatedAt=DateTime.Now,
            };
            var newRefreshTokenDto = new RefreshTokenDto
            {
                RefreshToken=newRefreshToken.Token,
            };
            _mapperMock.Setup(a=>a.Map<RefreshTokenDto>(It.IsAny<RefreshToken>())).Returns(newRefreshTokenDto);
            _mapperMock.Setup(a=>a.Map<UserDto>(It.IsAny<User>())).Returns(newUserDto);
            var result=await _userService.signUp(newUser.UserName,newUser.Email,newUser.PasswordHash,UserRole.Admin);
            Assert.NotNull(result);
            Assert.Equal(newUserDto.UserName,result.User.UserName);
            Assert.Equal(newRefreshTokenDto.RefreshToken,result.RefreshToken.RefreshToken);

        }
        [Fact]
        public async Task GetUser_ByEmail_ReturnUser()
        {
            var newUser = new User
            {
                Id = 1,
                UserName = "ammar",
                Email = "ammar@gmail.com",
                Role = UserRole.Admin.ToString(),
                CreatedAt = DateTime.Now,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("ammar123")
            };
            await _appDbContext.Users.AddAsync(newUser);
            await _appDbContext.SaveChangesAsync();
            var newUserDto = new UserDto
            {
                UserName = newUser.UserName,
                Email = newUser.Email,
                Role = UserRole.Admin.ToString(),
                CreatedAt = DateTime.Now,
            };
            _mapperMock.Setup(a=>a.Map<UserDto>(It.IsAny<User>())).Returns(newUserDto);
            var result = await _userService.getUserByEmail(newUser.Email);
            Assert.NotNull(result);
            Assert.Equal(newUserDto.UserName,result.UserName);
        }
        [Fact]
        public async Task SignIn_ByLoginRequest_ReturnUser()
        {
            var newUser = new User
            {
                Id = 1,
                UserName = "ammar",
                Email = "ammar@gmail.com",
                Role = UserRole.Admin.ToString(),
                CreatedAt = DateTime.Now,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("ammar123")
            };
            var newRefreshToken = new RefreshToken
            {
                Id = 1,
                User = newUser,
                UserId = newUser.Id,
                Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
                CreatedAt = DateTime.Now,
                ExpiredAt = DateTime.Now.AddSeconds(30),
            };
            await _appDbContext.Users.AddAsync(newUser);
            await _appDbContext.RefreshTokens.AddAsync(newRefreshToken);
            await _appDbContext.SaveChangesAsync();
            var newUserDto = new UserDto
            {
                UserName = newUser.UserName,
                Email = newUser.Email,
                Role = UserRole.Admin.ToString(),
                CreatedAt = DateTime.Now,
            };
            var newRefreshTokenDto = new RefreshTokenDto
            {
                RefreshToken = newRefreshToken.Token,
            };
            _mapperMock.Setup(a => a.Map<UserDto>(It.IsAny<User>())).Returns(newUserDto);
            _mapperMock.Setup(a => a.Map<RefreshTokenDto>(It.IsAny<RefreshToken>())).Returns(newRefreshTokenDto);
            var result = await _userService.signIn(newUser.Email,"ammar123");
            Assert.NotNull(result);
            Assert.Equal(newUserDto.UserName,result.User.UserName);
            Assert.Equal(newRefreshTokenDto.RefreshToken,result.RefreshToken.RefreshToken);
        }
        [Fact]
        public async Task GenerateJwtToken_ByUserEmail_ReturnJwtToken()
        {
            var newUser = new User
            {
                Id = 1,
                UserName = "ammar",
                Email = "ammar@gmail.com",
                Role = UserRole.Admin.ToString(),
                CreatedAt = DateTime.Now,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("ammar123")
            };
            await _appDbContext.Users.AddAsync(newUser);
            await _appDbContext.SaveChangesAsync();
            var result=await _userService.generateJwtToken(newUser.Email);
            Assert.NotNull(result);
        }
        [Fact]
        public async Task CreateRefreshToken_byUserEmail_ReturnUserRefreshToken()
        {
            var newUser = new User
            {
                Id = 1,
                UserName = "ammar",
                Email = "ammar@gmail.com",
                Role = UserRole.Admin.ToString(),
                CreatedAt = DateTime.Now,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("ammar123")
            };
            var newRefreshToken = new RefreshToken
            {
                Id = 1,
                User = newUser,
                UserId = newUser.Id,
                Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
                CreatedAt = DateTime.Now,
                ExpiredAt = DateTime.Now.AddSeconds(30),
            };
            await _appDbContext.Users.AddAsync(newUser);
            await _appDbContext.RefreshTokens.AddAsync(newRefreshToken);
            await _appDbContext.SaveChangesAsync();
            var newUserDto = new UserDto
            {
                UserName = newUser.UserName,
                Email = newUser.Email,
                Role = UserRole.Admin.ToString(),
                CreatedAt = DateTime.Now,
            };
            var newRefreshTokenDto = new RefreshTokenDto
            {
                RefreshToken = newRefreshToken.Token,
            };
            _mapperMock.Setup(a => a.Map<UserDto>(It.IsAny<User>())).Returns(newUserDto);
            _mapperMock.Setup(a => a.Map<RefreshTokenDto>(It.IsAny<RefreshToken>())).Returns(newRefreshTokenDto);
            var result = await _userService.createRefreshToken(newUser.Email);
            Assert.NotNull(result);
            Assert.Equal(newUser.Id,result.UserId);
        }
        [Fact]
        public async Task RefreshToken_ByUserEmail_ReturnRefreshToken()
        {
            var newUser = new User
            {
                Id = 1,
                UserName = "ammar",
                Email = "ammar@gmail.com",
                Role = UserRole.Admin.ToString(),
                CreatedAt = DateTime.Now,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("ammar123")
            };
            var newRefreshToken = new RefreshToken
            {
                Id = 1,
                User = newUser,
                UserId = newUser.Id,
                Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
                CreatedAt = DateTime.Now,
                ExpiredAt = DateTime.Now.AddSeconds(30),
            };
            await _appDbContext.Users.AddAsync(newUser);
            await _appDbContext.RefreshTokens.AddAsync(newRefreshToken);
            await _appDbContext.SaveChangesAsync();
            var newUserDto = new UserDto
            {
                UserName = newUser.UserName,
                Email = newUser.Email,
                Role = UserRole.Admin.ToString(),
                CreatedAt = DateTime.Now,
            };
            var newRefreshTokenDto = new RefreshTokenDto
            {
                RefreshToken = newRefreshToken.Token,
            };
            var newSigningResponse = new SigningResponse
            {
                User=newUserDto,
                RefreshToken=newRefreshTokenDto
            };
            _mapperMock.Setup(a => a.Map<UserDto>(It.IsAny<User>())).Returns(newUserDto);
            _mapperMock.Setup(a => a.Map<RefreshTokenDto>(It.IsAny<RefreshToken>())).Returns(newRefreshTokenDto);
            var result = await _userService.refreshToken(newUser.Email);
            Assert.NotNull(result);
            Assert.Equal(newSigningResponse.User.Email, result.User.Email);
            Assert.Equal(newSigningResponse.RefreshToken.RefreshToken, result.RefreshToken.RefreshToken);
        }
        [Fact]
        public async Task GetCurrentUser_returnUser()
        {
            var newUser = new User
            {
                Id = 1,
                UserName = "ammar",
                Email = "ammar@gmail.com",
                Role = UserRole.Admin.ToString(),
                CreatedAt = DateTime.Now,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("ammar123")
            };
            await _appDbContext.Users.AddAsync(newUser);
            await _appDbContext.SaveChangesAsync();
            var newUserDto = new UserDto
            {
                UserName = newUser.UserName,
                Email = newUser.Email,
                Role = UserRole.Admin.ToString(),
                CreatedAt = DateTime.Now,
            };
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, newUser.Email)
            };

            var identity = new ClaimsIdentity(claims);

            var claimsPrincipal = new ClaimsPrincipal(identity);

            var context = new DefaultHttpContext
            {
                User = claimsPrincipal
            };

            _contextAccessorMock.Setup(x => x.HttpContext).Returns(context);
            var result = await _userService.getCurrentUser();
            Assert.NotNull(result);
            Assert.Equal(newUserDto.Email, result.Email);
        }
        [Fact]
        public async Task SignOut_ExpireRefreshToken()
        {
            var newUser = new User
            {
                Id = 1,
                UserName = "ammar",
                Email = "ammar@gmail.com",
                Role = UserRole.Admin.ToString(),
                CreatedAt = DateTime.Now,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("ammar123")
            };
            var newRefreshToken = new RefreshToken
            {
                Id = 1,
                User = newUser,
                UserId = newUser.Id,
                Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
                CreatedAt = DateTime.Now,
                ExpiredAt = DateTime.Now.AddSeconds(30),
            };
            await _appDbContext.Users.AddAsync(newUser);
            await _appDbContext.RefreshTokens.AddAsync(newRefreshToken);
            await _appDbContext.SaveChangesAsync();
            var newUserDto = new UserDto
            {
                UserName = newUser.UserName,
                Email = newUser.Email,
                Role = UserRole.Admin.ToString(),
                CreatedAt = DateTime.Now,
            };
            var newRefreshTokenDto = new RefreshTokenDto
            {
                RefreshToken = newRefreshToken.Token,
            };
            var newSigningResponse = new SigningResponse
            {
                User = newUserDto,
                RefreshToken = newRefreshTokenDto
            };
            _mapperMock.Setup(a => a.Map<UserDto>(It.IsAny<User>())).Returns(newUserDto);
            _mapperMock.Setup(a => a.Map<RefreshTokenDto>(It.IsAny<RefreshToken>())).Returns(newRefreshTokenDto);
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, newUser.Email)
            };
            var identity=new ClaimsIdentity(claims);    
            var claimsPrincipal=new ClaimsPrincipal(identity);
            var context = new DefaultHttpContext
            {
                User = claimsPrincipal
            };
            _contextAccessorMock.Setup(a => a.HttpContext).Returns(context);
            await _userService.signOut();
            var expiredToken=await _appDbContext.RefreshTokens.OrderByDescending(a=>a.CreatedAt).FirstOrDefaultAsync(a=>a.Token==newRefreshTokenDto.RefreshToken);
            Assert.NotNull(expiredToken);
            Assert.Equal(false,expiredToken.isValid);
        }
    }
}
