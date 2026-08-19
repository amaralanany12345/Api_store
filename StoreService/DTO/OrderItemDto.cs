using StoreDomain.Models;

namespace StoreService.DTO
{
    public class OrderItemDto
    {
        public int ItemId { get; set; }
        public string ItemName { get; set; }
        public int Price { get; set; }
        public int Quantity { get; set; }
    }
}
