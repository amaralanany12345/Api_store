namespace StoreWebApi.Models
{
    public class Item
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Price { get; set; }
        public int StockQuantity { get; set; }
        public Category Category { get; set; }
        public int CategoryId { get; set; }
        public List<OrderItem> OrderItems { get; set; }

    }
}
