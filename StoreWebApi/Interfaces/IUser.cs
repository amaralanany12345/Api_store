using StoreWebApi.Enums;
using StoreWebApi.Models;

namespace StoreWebApi.Interfaces
{
    public interface IUser
    {
        Task<SigningResponse> signUp(string userName,string email,string password,UserRole role);
        Task<SigningResponse> signIn(string userName,string password);
        Task signOut();
        Task<User> getUserByEmail(string email);
        Task<string> generateJwtToken(string userEmail);
        string generateRandomRefreshToken();
        Task<RefreshToken> createRefreshToken(string userEmail);
        Task<SigningResponse> refreshToken(string userEmail);
        Task<User> getCurrentUser();

    }
}
