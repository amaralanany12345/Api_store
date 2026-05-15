using Microsoft.AspNetCore.Authorization;

namespace StoreWebApi.Policies
{
    public class ValidRefreshToken : IAuthorizationRequirement
    {
    }
}
