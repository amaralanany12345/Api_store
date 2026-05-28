using StoreService.DTO;
namespace StoreService.ResponseModel
{
    public class SigningResponse
    {
        public UserDto User { get; set; }
        public string jwtToken { get; set; }
        public RefreshTokenDto RefreshToken { get; set; }
    }
}
