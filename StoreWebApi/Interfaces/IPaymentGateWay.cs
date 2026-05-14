using StoreWebApi.DTO;
using StoreWebApi.Models;

namespace StoreWebApi.Interfaces
{
    public interface IPaymentGateWay
    {
        Task<ReceiptDto> payForOrder();
    }
}
