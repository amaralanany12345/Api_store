using StoreWebApi.Enums;

namespace StoreWebApi.Models
{
    public class RegisterRequest
    {
        public string userName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public UserRole Role { get; set; }
    }
}
