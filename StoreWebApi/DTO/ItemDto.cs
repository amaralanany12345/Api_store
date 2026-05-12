using System.ComponentModel.DataAnnotations;

namespace StoreWebApi.DTO
{
    public class ItemDto
    {
        [Required]
        [MaxLength(255)]
        public string Name { get; set; }
        [Required]
        public int Price { get; set; }
        [Required]
        public int StockQuantity { get; set; }
        [Required]
        public string CategoryName { get; set; }
    }
}
