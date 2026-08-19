namespace StoreService.DTO
{
    public class ReceiptDto
    {
        public int OrderId { get; set; }
        public int TotalAmount { get; set; }
        public DateTime CreateAt { get; set; }
    }
}
