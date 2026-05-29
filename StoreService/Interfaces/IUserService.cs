using StoreDomain.Enums;
using StoreDomain.Models;
using StoreService.ResponseModel;


namespace StoreService.Interfaces
{
    public interface IUserService
    {
        Task<ResultResponse<SigningResponse>> SignUp(string userName,string email,string password,UserRole role);
        Task<ResultResponse<SigningResponse>> SignIn(string userEmail,string password);
        Task SignOut();
        Task<ResultResponse<User>> GetUserByEmail(string email);
        Task<string> GenerateJwtToken(string userEmail);
        string GenerateRandomRefreshToken();
        Task<RefreshToken> CreateRefreshToken(string userEmail);
        Task<ResultResponse<SigningResponse>> RefreshToken(string userEmail);
        Task<ResultResponse<User>> GetCurrentUser();

    }
}
