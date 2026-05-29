using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using StoreDataBase.AppContexts;
using StoreDomain.Models;
using StoreService.DTO;
using StoreService.RepositoriesInterfaces;
using StoreService.ResponseModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoreDataBase.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;
        public UserRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<RefreshToken> GetLastRefreshToken(int userId)
        {
            return await _context.RefreshTokens.Where(a => a.UserId == userId).OrderByDescending(a => a.CreatedAt).FirstOrDefaultAsync();
        }
    }
}
