using AutoMapper;
using Microsoft.EntityFrameworkCore;
using StoreWebApi.DTO;
using StoreWebApi.Interfaces;
using StoreWebApi.Models;

namespace StoreWebApi.Services
{
    public class ItemService : IItem
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        public ItemService(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<ItemDto> createItem(string name, int price, int stockQuantity, int categoryId)
        {
            var category=await _context.Categories.Where(a=>a.Id == categoryId).FirstOrDefaultAsync();
            if(category == null)
            {
                throw new ArgumentException("category is not found");
            }
            var newItem=new Item
            {
                Name = name,
                Price = price,
                StockQuantity = stockQuantity,
                CategoryId = categoryId,
                Category = category

            };
            await _context.Items.AddAsync(newItem);
            await _context.SaveChangesAsync();
            return _mapper.Map<ItemDto>(newItem);
        }

        public async Task deleteItem(int ItemId)
        {
            var item = await getItemById(ItemId);
            _context.Items.Remove(item);
            await _context.SaveChangesAsync();

        }
        private async Task<Item> getItemById(int itemId)
        {
            var item=await _context.Items.Where(a=>a.Id==itemId).FirstOrDefaultAsync();
            if(item == null)
            {
                throw new ArgumentException("Item is not found");
            }
            return item;    
        }

        public async Task<ItemDto> getITem(string name)
        {
            var item = await _context.Items.Where(a => a.Name == name).FirstOrDefaultAsync();
            if (item == null)
            {
                throw new ArgumentException("Item is not found");
            }
            return _mapper.Map<ItemDto>(item);
        }

        public async Task<List<ItemDto>> getAllItems()
        {
            return _mapper.Map<List<ItemDto>>(await _context.Items.ToListAsync());
        }

        public async Task<ItemDto> updateItem(int ItemId, string newName, int newPrice, int stockQuantity)
        {
            var item=await getItemById(ItemId);
            item.Name = newName;
            item.Price = newPrice;
            item.StockQuantity = stockQuantity;
            await _context.SaveChangesAsync();
            return _mapper.Map<ItemDto>(item);
        }

        public async Task<List<ItemDto>> getITemByCategoryName(string categoryName)
        {
            var category=await _context.Categories.Where(a=>a.Name==categoryName).FirstOrDefaultAsync();
            if(category == null)
            {
                throw new ArgumentException("category is not found");
            }
            return _mapper.Map<List<ItemDto>>(await _context.Items.Where(a=>a.Category.Name==categoryName).ToListAsync());
            
        }
    }
}
