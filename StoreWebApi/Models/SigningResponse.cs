using StoreWebApi.DTO;

namespace StoreWebApi.Models
{
    public class SigningResponse
    {
        public UserDto User { get; set; }
        public string jwtToken { get; set; }
        public RefreshTokenDto RefreshToken { get; set; }
    }
}
