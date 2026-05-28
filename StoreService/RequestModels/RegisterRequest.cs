using StoreDomain.Enums;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace StoreService.RequestModels
{
    public class RegisterRequest
    {
        [Required]
        public string userName { get; set; }
        [EmailAddress]
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public UserRole Role { get; set; }
    }
}
