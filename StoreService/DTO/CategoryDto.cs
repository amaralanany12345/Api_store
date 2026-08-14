using System.ComponentModel.DataAnnotations;

namespace StoreService.DTO
{
    public class CategoryDto
    {
        public int Id { get; set; }
        [Required]
        [MaxLength(255)]
        public string Name { get; set; }
        [Required]
        [MaxLength (1500)]
        public string Description { get; set; }
    }
}
