using StoreWebApi.Enums;
using StoreWebApi.Models;

namespace StoreWebApi.Interfaces
{
    public interface IUser
    {
        Task<SigningResponse> signUp(string userName,string email,string password,UserRole role,int? balance);
        Task<SigningResponse> signIn(string userName,string password);
        Task signOut();
        Task<User> getUserByEmail(string email);
        Task<string> generateJwtToken(int userId);
        string generateRandomRefreshToken();
        Task<RefreshToken> createRefreshToken(int userId);
        Task<SigningResponse> refreshToken(int userId);
        Task<User> getCurrentUser();

    }
}
