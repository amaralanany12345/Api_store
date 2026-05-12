namespace StoreWebApi.Models
{
    public class Receipt
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public int TotalAmount { get; set; }
        public Order Order { get; set; }
        public int orderId { get; set; }
    }
}
