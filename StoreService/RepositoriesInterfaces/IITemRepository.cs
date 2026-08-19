using StoreDomain.Models;
using StoreService.DTO;
using StoreService.ResponseModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoreService.RepositoriesInterfaces
{
    public interface IITemRepository
    {
        Task<List<Item>> GetITemByCategory(int categoryId, int pageSize, int pageNumber);
        Task<List<Item>> GetItemsByCategoryId(int categoryId);
        Task<List<Item>> SearchByName(string itemName);

    }
}
