using Microsoft.AspNetCore.Authentication;
using Ratbags.Accounts.API.Models.API.Tokens;

namespace Ratbags.Accounts.API.Interfaces
{
    public interface IExternalSigninService
    {
        Task<RefreshTokenAndJWTOrchestratorResponse?> Signin(AuthenticateResult authenticateResult, string providerName);
    }
}