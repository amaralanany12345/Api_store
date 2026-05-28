using StoreDomain.Enums;
using StoreDomain.Models;
using StoreService.ResponseModel;


namespace StoreService.Interfaces
{
    public interface IUserService
    {
        Task<SigningResponse> SignUp(string userName,string email,string password,UserRole role);
        Task<SigningResponse> SignIn(string userEmail,string password);
        Task SignOut();
        Task<User> GetUserByEmail(string email);
        Task<string> GenerateJwtToken(string userEmail);
        string GenerateRandomRefreshToken();
        Task<RefreshToken> CreateRefreshToken(string userEmail);
        Task<SigningResponse> RefreshToken(string userEmail);
        Task<User> GetCurrentUser();

    }
}
