using AutoMapper;
using Microsoft.AspNetCore.Http;
using StoreService.DTO;
using StoreService.Interfaces;
using StoreService.ResponseModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoreService.Services
{
    public class ReceiptService : IReceiptService
    {
        private readonly IUnitOfWorkServiceForStoreDb _unitOfWorkService;
        private readonly IMapper _mapper;
        public ReceiptService(IUnitOfWorkServiceForStoreDb unitOfWorkService, IMapper mapper)
        {
            _unitOfWorkService = unitOfWorkService;
            _mapper = mapper;
        }

        public async Task<ResultResponse<List<ReceiptDto>>> GetAllReceipts()
        {
            return ResultResponse<List<ReceiptDto>>.Pass(_mapper.Map<List<ReceiptDto>>(await _unitOfWorkService.Receipts.GetAllAsync()), StatusCodes.Status200OK);
        }

        public async Task<ResultResponse<ReceiptDto>> GetReceipt(int orderId)
        {
            return ResultResponse<ReceiptDto>.Pass(_mapper.Map<ReceiptDto>(await _unitOfWorkService.Receipts.GetFirstOrDefault(a => a.orderId == orderId)), StatusCodes.Status200OK);

        }
    }
}
