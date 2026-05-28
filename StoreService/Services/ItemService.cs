using AutoMapper;
using StoreService.DTO;
using StoreService.Interfaces;
using StoreDomain.Models;
using Microsoft.Extensions.Logging;

namespace StoreService.Services
{
    public class ItemService : IItemService
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWorkServiceForStoreDb _unitOfWork;
        private readonly ILogger<ItemService> _logger;
        public ItemService(IMapper mapper, IUnitOfWorkServiceForStoreDb unitOfWork, ILogger<ItemService> logger)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }
        public async Task<ItemDto> CreateItem(string name, int price, int stockQuantity, string categoryName)
        {
            var category = await _unitOfWork.Categories.GetFirstOrDefault(a=>a.Name==categoryName);
            var newItem=new Item
            {
                Name = name,
                Price = price,
                StockQuantity = stockQuantity,
                CategoryId = category.Id,
                Category = category
            };
            await _unitOfWork.Items.CreateAsync(newItem);
            await _unitOfWork.SaveChangesAsync();
            _logger.LogInformation($"item is created with name{newItem.Name} and it belong to category {category.Name}");
            return _mapper.Map<ItemDto>(newItem);
        }

        public async Task DeleteItem(int itemId)
        {
            _unitOfWork.Items.DeleteAsync(await _unitOfWork.Items.GetAsync(itemId));
            await _unitOfWork.SaveChangesAsync();

        }
        public async Task<ItemDto> GetITem(int itemId)
        {
            var item=await _unitOfWork.Items.GetAsync(itemId);
            return _mapper.Map<ItemDto>(item);
        }

        public async Task<List<ItemDto>> GetAllItems()
        {
            return _mapper.Map<List<ItemDto>>(await _unitOfWork.Items.GetAllAsync());
        }

        public async Task<ItemDto> UpdateItem(int itemId, string newName, int newPrice, int stockQuantity)
        {
            var item= await GetITem(itemId);
            item.Name = newName;
            item.Price = newPrice;
            item.StockQuantity = stockQuantity;
            await _unitOfWork.SaveChangesAsync();
            return _mapper.Map<ItemDto>(item);
        }

        public async Task<List<ItemDto>> GetITemByCategory(int categoryId, int pageSize, int pageNumber)
        {
        return _mapper.Map<List<ItemDto>>(await _unitOfWork.ITemRepository.GetITemByCategory(categoryId,pageSize,pageNumber));

        }

    }
}
