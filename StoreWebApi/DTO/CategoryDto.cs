using System.ComponentModel.DataAnnotations;

namespace StoreWebApi.DTO
{
    public class CategoryDto
    {
        [Required]
        [MaxLength(255)]
        public string Name { get; set; }
        [Required]
        [MaxLength (1500)]
        public string Description { get; set; }
    }
}
