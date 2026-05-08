using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using StoreWebApi.DTO;
using StoreWebApi.Enums;
using StoreWebApi.Interfaces;
using StoreWebApi.Models;
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
        public UserService(AppDbContext context, Jwt jwt, IMapper mapper)
        {
            _context = context;
            _jwt = jwt;
            _mapper = mapper;
        }
        public async Task<SigningResponse> signUp(string userName, string email, string password, UserRole role, int? balance)
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
                newUser.Balance = balance;
            }
            await _context.Users.AddAsync(newUser);
            await _context.SaveChangesAsync();
            return new SigningResponse
            {
                User = _mapper.Map<UserDto>(newUser),
                jwtToken = await generateJwtToken(newUser.Id),
                RefreshToken= _mapper.Map<RefreshTokenDto>(await createRefreshToken(newUser.Id))
            };
        }
        public async Task<SigningResponse> signIn(string userName, string password)
        {
            var user = await getUserByEmail(userName);
            if (user == null || !(BCrypt.Net.BCrypt.Verify(password, user.PasswordHash)))
            {
                throw new ArgumentException("user is not found");
            }
            return new SigningResponse
            {
                User = _mapper.Map<UserDto>(user),
                jwtToken = await generateJwtToken(user.Id),
                RefreshToken= _mapper.Map<RefreshTokenDto>(await createRefreshToken(user.Id))
            };
        }
        public async Task<User> getUserByEmail(string email)
        {
            var user = await _context.Users.Where(a => a.Email == email).FirstOrDefaultAsync();
            if (user == null)
            {
                throw new ArgumentException("user is not found");
            }
            return user;
        }
        public async Task<string> generateJwtToken(int userId)
        {
            var user = await getUserById(userId);
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Issuer = _jwt.Issuer,
                Audience = _jwt.Audience,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Signingkey)),
                SecurityAlgorithms.HmacSha256Signature),
                Subject = new System.Security.Claims.ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.NameIdentifier,user.Id.ToString()),
                    new Claim(ClaimTypes.Name,user.UserName),
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

        public async Task<RefreshToken> createRefreshToken(int userId)
        {
            var user=await getUserById(userId);
            var newRefreshToken = new RefreshToken
            {
                User=user,
                UserId=userId,
                Token=generateRandomRefreshToken(),
                CreatedAt=DateTime.Now,
                ExpiredAt=DateTime.Now.AddSeconds(10),
            };
            await _context.RefreshTokens.AddAsync(newRefreshToken);
            await _context.SaveChangesAsync();
            return newRefreshToken;
        }
        public async Task<SigningResponse> refreshToken(int userId)
        {
            var user=_mapper.Map<UserDto>(await getUserById(userId));
            var refreshToken=await _context.RefreshTokens.Where(a=>a.UserId==userId).OrderByDescending(a=>a.CreatedAt).FirstOrDefaultAsync();
            if(refreshToken==null || !refreshToken.isValid)
            {
                throw new ArgumentException("your refresh token is expired");
            }
            refreshToken.Token=generateRandomRefreshToken();
            refreshToken.CreatedAt=DateTime.Now;
            refreshToken.ExpiredAt=DateTime.Now.AddSeconds(10);
            await _context.SaveChangesAsync();
            return new SigningResponse
            {
                User=user,
                jwtToken= await generateJwtToken(userId),
                RefreshToken=_mapper.Map<RefreshTokenDto>(refreshToken),    
            };
        }
        public Task signOut()
        {
            throw new NotImplementedException();
        }
        private async Task<User> getUserById(int userId)
        {
            var user = await _context.Users.Where(a => a.Id == userId).FirstOrDefaultAsync();
            if(user == null)
            {
                throw new ArgumentException("user is not found");
            }
            return user;
        }
    }
}
