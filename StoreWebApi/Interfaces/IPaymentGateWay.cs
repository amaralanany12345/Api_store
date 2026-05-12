using StoreWebApi.Models;

namespace StoreWebApi.Interfaces
{
    public interface IPaymentGateWay
    {
        Task<Receipt> payForOrder(int orderId);
    }
}
