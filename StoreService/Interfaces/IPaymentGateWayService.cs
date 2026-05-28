using StoreService.DTO;
using StoreDomain.Models;

namespace StoreService.Interfaces
{
    public interface IPaymentGateWayService
    {
        Task<ReceiptDto> PayForOrder();
    }
}
