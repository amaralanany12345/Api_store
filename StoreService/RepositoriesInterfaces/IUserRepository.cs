using StoreDomain.Models;
using StoreService.ResponseModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoreService.RepositoriesInterfaces
{
    public interface IUserRepository
    {
        Task<RefreshToken> GetLastRefreshToken(int userId);
    }
}
