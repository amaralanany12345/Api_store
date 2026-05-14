using AutoMapper;
using Microsoft.EntityFrameworkCore;
using StoreWebApi.DTO;
using StoreWebApi.Interfaces;
using StoreWebApi.Models;
using StoreWebApi.zAppContexts;

namespace StoreWebApi.Services
{
    public class ItemService : IItem
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        private readonly IGenericRepo<Item> _genericRepo;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<ItemService> _logger;
        public ItemService(AppDbContext context, IMapper mapper, IGenericRepo<Item> genericRepo, IUnitOfWork unitOfWork, ILogger<ItemService> logger)
        {
            _context = context;
            _mapper = mapper;
            _genericRepo = genericRepo;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }
        public async Task<ItemDto> createItem(string name, int price, int stockQuantity, string categoryName)
        {
            var category=await _context.Categories.Where(a=>a.Name == categoryName).FirstOrDefaultAsync();
            if(category == null)
            {
                _logger.LogWarning("category is not found so you can't create item");
                throw new ArgumentException("category is not found so you can't create item");
            }
            var newItem=new Item
            {
                Name = name,
                Price = price,
                StockQuantity = stockQuantity,
                CategoryId = category.Id,
                Category = category
            };
            await _genericRepo.CreateAsync(newItem);
            await _unitOfWork.saveChangesAsync();
            _logger.LogInformation($"item is created with name{newItem.Name} and it belong to category {category.Name}");
            return _mapper.Map<ItemDto>(newItem);
        }

        public async Task deleteItem(string ItemName)
        {
            var item = await getITem(ItemName);
            _context.Items.Remove(_mapper.Map<Item>(item));
            await _context.SaveChangesAsync();

        }
        public async Task<ItemDto> getITem(string name)
        {
            var item = await _context.Items.Where(a => a.Name == name).FirstOrDefaultAsync();
            if (item == null)
            {
                _logger.LogInformation("Item is not found with this name ");
                throw new ArgumentException("Item is not found with this name");
            }
            return _mapper.Map<ItemDto>(item);
        }

        public async Task<List<ItemDto>> getAllItems()
        {
            return _mapper.Map<List<ItemDto>>(await _context.Items.ToListAsync());
        }

        public async Task<ItemDto> updateItem(string itemName, string newName, int newPrice, int stockQuantity)
        {
            var item=await getITem(itemName);
            item.Name = newName;
            item.Price = newPrice;
            item.StockQuantity = stockQuantity;
            await _context.SaveChangesAsync();
            return _mapper.Map<ItemDto>(item);
        }

        public async Task<List<ItemDto>> getITemByCategoryName(string categoryName,int pageSize,int pageNumber)
        {
            var category=await _context.Categories.Where(a=>a.Name==categoryName).FirstOrDefaultAsync();
            if(category == null)
            {
                _logger.LogInformation("category is not found with this name");
                throw new ArgumentException("category is not found");
            }

            return _mapper.Map<List<ItemDto>>(await _context.Items.Where(a=>a.Category.Name==categoryName)
                .Skip((pageNumber-1)*pageSize).Take(pageSize).ToListAsync());
            
        }
    }
}
