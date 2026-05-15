using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using StoreWebApi.DTO;
using StoreWebApi.Enums;
using StoreWebApi.Interfaces;
using StoreWebApi.Models;
using StoreWebApi.zAppContexts;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace StoreWebApi.Services
{
    public class UserService:IUser
    {
        private readonly AppDbContext _context;
        private readonly Jwt _jwt;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IGenericRepo<User> _genericRepo;
        private readonly ILogger<UserService> _logger;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IWallet _walletService;
        public UserService(AppDbContext context, Jwt jwt, IMapper mapper, IUnitOfWork unitOfWork, IGenericRepo<User> genericRepo, ILogger<UserService> logger, IHttpContextAccessor contextAccessor, IWallet walletService)
        {
            _context = context;
            _jwt = jwt;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _genericRepo = genericRepo;
            _logger = logger;
            _contextAccessor = contextAccessor;
            _walletService = walletService;
        }
        public async Task<SigningResponse> signUp(string userName, string email, string password, UserRole role)
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
                await _walletService.createWalletToUser(email);
                var newUserWallet=await _walletService.getWalletOfUser(email);
                newUser.Balance = newUserWallet.Balance;
            }
            else
            {
                newUser.Balance = null;
            }
            await _genericRepo.CreateAsync(newUser);
            await _unitOfWork.saveChangesAsync();
            return new SigningResponse
            {
                User = _mapper.Map<UserDto>(newUser),
                jwtToken = await generateJwtToken(newUser.Email),
                RefreshToken= _mapper.Map<RefreshTokenDto>(await createRefreshToken(newUser.Email))
            };
        }
        public async Task<SigningResponse> signIn(string userName, string password)
        {
            var user = await getUserByEmail(userName);
            if (user == null || !(BCrypt.Net.BCrypt.Verify(password, user.PasswordHash)))
            {
                _logger.LogWarning("your email or password is not found");
                throw new ArgumentException("user is not found");
            }
            return new SigningResponse
            {
                User = _mapper.Map<UserDto>(user),
                jwtToken = await generateJwtToken(user.Email),
                RefreshToken= _mapper.Map<RefreshTokenDto>(await createRefreshToken(user.Email))
            };
        }
        public async Task<User> getUserByEmail(string email)
        {
            var user = await _context.Users.Where(a => a.Email == email).FirstOrDefaultAsync();
            if (user == null)
            {
                _logger.LogWarning("user is not found with this email");
                throw new ArgumentException("user is not found");
            }
            return user;
        }
        public async Task<string> generateJwtToken(string userEmail)
        {
            var user = await getUserByEmail(userEmail);
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
        public string generateRandomRefreshToken()
        {
            return Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        }

        public async Task<RefreshToken> createRefreshToken(string userEmail)
        {
            var user=await getUserByEmail(userEmail);
            var newRefreshToken = new RefreshToken
            {
                User=user,
                UserId=user.Id,
                Token=generateRandomRefreshToken(),
                CreatedAt=DateTime.Now,
                ExpiredAt=DateTime.Now.AddSeconds(20),
            };
            await _context.RefreshTokens.AddAsync(newRefreshToken);
            await _context.SaveChangesAsync();
            return newRefreshToken;
        }
        public async Task<SigningResponse> refreshToken(string userEmail)
        {
            var user=await getUserByEmail(userEmail);
            var refreshToken=await _context.RefreshTokens.Where(a=>a.UserId==user.Id).OrderByDescending(a=>a.CreatedAt).FirstOrDefaultAsync();
            if(refreshToken==null || !refreshToken.isValid)
            {
                _logger.LogInformation("your refresh token is expired");
                //throw new ArgumentException("your refresh token is expired");
            }
            refreshToken.Token=generateRandomRefreshToken();
            refreshToken.CreatedAt=DateTime.Now;
            refreshToken.ExpiredAt=DateTime.Now.AddMinutes(3);
            await _context.SaveChangesAsync();
            return new SigningResponse
            {
                User=_mapper.Map<UserDto>(user),
                jwtToken= await generateJwtToken(userEmail),
                RefreshToken=_mapper.Map<RefreshTokenDto>(refreshToken),    
            };
        }
        public async Task<User> getCurrentUser()
        {
            var currentUserEmail = _contextAccessor.HttpContext?.User.FindFirst(ClaimTypes.Email)?.Value;
            if (currentUserEmail == null)
            {
                throw new ArgumentException("user is not found");
            }
            return await getUserByEmail(currentUserEmail);
        }
        public async Task signOut()
        {
            var user=await getCurrentUser();
            var refreshToken=await _context.RefreshTokens.Where(a=>a.UserId==user.Id).OrderByDescending(A=>A.CreatedAt).FirstOrDefaultAsync();
            if(refreshToken == null)
            {
                throw new ArgumentException("your token is not found");
            }
            refreshToken.ExpiredAt = DateTime.Now;
            await _context.SaveChangesAsync();
        }
        
    }
}
