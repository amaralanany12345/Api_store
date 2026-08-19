using Microsoft.EntityFrameworkCore;
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
    public class ItemRepository : IITemRepository
    {
        private readonly AppDbContext _context;
        public ItemRepository(AppDbContext context) 
        {
            _context = context;
        }

        public async Task<List<Item>> GetITemByCategory(int categoryId, int pageSize, int pageNumber)
        {
            var items = await _context.Items.Where(a=>a.CategoryId==categoryId).Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

            return items;
        }

        public async Task<List<Item>> GetItemsByCategoryId(int categoryId)
        {

            var items = await _context.Items.Where(a => a.CategoryId == categoryId).ToListAsync();
            return items;
        }

        public async Task<List<Item>> SearchByName(string itemName)
        {
            var item=await _context.Items.Where(a=>a.Name.Contains(itemName)).ToListAsync();
            return item;
        }
    }
}
