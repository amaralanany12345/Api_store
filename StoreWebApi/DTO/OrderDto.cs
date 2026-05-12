namespace StoreWebApi.DTO
{
    public class OrderDto
    {
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string Status { get; set; }
        public int TotalAmount { get; set; }
    }
}
