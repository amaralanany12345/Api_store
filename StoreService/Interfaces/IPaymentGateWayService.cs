using StoreService.DTO;
using StoreDomain.Models;
using StoreService.ResponseModel;

namespace StoreService.Interfaces
{
    public interface IPaymentGateWayService
    {
        Task<ResultResponse<ReceiptDto>> PayForOrder();
    }
}
