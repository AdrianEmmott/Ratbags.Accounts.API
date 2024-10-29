using Microsoft.AspNetCore.Authentication;
using Ratbags.Accounts.API.Models.API;
using Ratbags.Accounts.API.Models.DB;

namespace Ratbags.Accounts.API.Interfaces
{
    public interface IExternalSigninService
    {
        Task<RefreshTokenAndJWTResponse?> Signin(AuthenticateResult authenticateResult, string providerName);
    }
}