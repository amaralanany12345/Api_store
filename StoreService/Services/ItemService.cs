using AutoMapper;
using StoreService.DTO;
using StoreService.Interfaces;
using StoreDomain.Models;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using StoreService.ResponseModel;
using StoreDomain.Enums;

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
        public async Task<ResultResponse<ItemDto>> CreateItem(string name, int price, int stockQuantity, string categoryName)
        {
            var category = await _unitOfWork.Categories.GetFirstOrDefault(a=>a.Name==categoryName);
            if (category == null)
            {
                return ResultResponse<ItemDto>.Fail("category is not found",ErrorTypes.NotFound,StatusCodes.Status404NotFound);
            }
            var newItem=new Item
            {
                Name = name,
                Price = price,
                StockQuantity = stockQuantity,
                CategoryId = category.Id,
                Category = category
            };
            var existITem=await _unitOfWork.Items.GetFirstOrDefault(a=>a.Name == newItem.Name);
            if (existITem != null)
            {
                return ResultResponse<ItemDto>.Fail("item is already found", ErrorTypes.Conflict, StatusCodes.Status409Conflict);
            }
            await _unitOfWork.Items.CreateAsync(newItem);
            await _unitOfWork.SaveChangesAsync();
            _logger.LogInformation($"item is created with name{newItem.Name} and it belong to category {category.Name}");
            return ResultResponse<ItemDto>.Pass(_mapper.Map<ItemDto>(newItem),StatusCodes.Status201Created);
        }

        public async Task DeleteItem(int itemId)
        {
            await _unitOfWork.Items.DeleteAsync(itemId);
            await _unitOfWork.SaveChangesAsync();

        }
        public async Task<ResultResponse<ItemDto>> GetITem(int itemId)
        {
            var item=await _unitOfWork.Items.GetAsync(itemId);
            if (item == null)
            {
                return ResultResponse<ItemDto>.Fail("item is not found",ErrorTypes.NotFound,StatusCodes.Status404NotFound);
            }
            return ResultResponse<ItemDto>.Pass(_mapper.Map<ItemDto>(item),StatusCodes.Status200OK);
        }

        public async Task<ResultResponse<List<ItemDto>>> GetAllItems()
        {
            return ResultResponse<List<ItemDto>>.Pass(_mapper.Map<List<ItemDto>>(await _unitOfWork.Items.GetAllAsync()),StatusCodes.Status200OK);
        }

        public async Task<ResultResponse<ItemDto>> UpdateItem(int itemId, string newName, int newPrice, int stockQuantity)
        {
            var item= await _unitOfWork.Items.GetAsync(itemId);
            if (item == null)
            {
                return ResultResponse<ItemDto>.Fail("item is not found", ErrorTypes.NotFound, StatusCodes.Status404NotFound);
            }
            item.Name = newName;
            item.Price = newPrice;
            item.StockQuantity = stockQuantity;
            await _unitOfWork.SaveChangesAsync();
            return ResultResponse<ItemDto>.Pass(_mapper.Map<ItemDto>(item), StatusCodes.Status200OK);
        }

        public async Task<ResultResponse<List<ItemDto>>> GetITemByCategory(int categoryId, int pageSize, int pageNumber)
        {
            var category=await _unitOfWork.Categories.GetAsync(categoryId);
            if (category==null)
            {
                return ResultResponse<List<ItemDto>>.Fail("category is not found", ErrorTypes.NotFound, StatusCodes.Status404NotFound);
            }
            return ResultResponse<List<ItemDto>>.Pass(_mapper.Map<List<ItemDto>>(await _unitOfWork.ITemRepository.GetITemByCategory(categoryId, pageSize, pageNumber)),StatusCodes.Status200OK);
        }

        public async Task<ResultResponse<List<ItemDto>>> GetItemsByCategoryId(int categoryId)
        {
            var category = await _unitOfWork.Categories.GetAsync(categoryId);
            if (category == null)
            {
                return ResultResponse<List<ItemDto>>.Fail("category is not found", ErrorTypes.NotFound, StatusCodes.Status404NotFound);
            }
            return ResultResponse<List<ItemDto>>.Pass(_mapper.Map<List<ItemDto>>(await _unitOfWork.ITemRepository.GetItemsByCategoryId(categoryId)), StatusCodes.Status200OK);

        }

        public async Task<ResultResponse<List<ItemDto>>> SearchByName(string itemName)
        {
            return ResultResponse<List<ItemDto>>.Pass(_mapper.Map<List<ItemDto>>(await _unitOfWork.ITemRepository.SearchByName(itemName)),StatusCodes.Status200OK);
        }
    }
}
