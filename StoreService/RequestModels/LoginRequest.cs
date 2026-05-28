using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace StoreService.RequestModels
{
    public class LoginRequest
    {
        [EmailAddress]
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
