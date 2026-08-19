using StoreService.DTO;
using StoreService.ResponseModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoreService.Interfaces
{
    public interface IReceiptService
    {
        Task<ResultResponse<ReceiptDto>> GetReceipt(int orderId);
        Task<ResultResponse<List<ReceiptDto>>> GetAllReceipts();
    }
}
