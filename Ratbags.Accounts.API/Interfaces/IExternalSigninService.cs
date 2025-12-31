using Microsoft.AspNetCore.Authentication;
using Ratbags.Accounts.API.Models.Accounts.ExternalSignIn;
using Ratbags.Accounts.API.Models.API.Tokens;
using Ratbags.Accounts.API.Models.DB;
using System.Security.Claims;

namespace Ratbags.Accounts.API.Interfaces
{
    public interface IExternalSigninService
    {
        Task<RefreshTokenAndJWTOrchestratorResponse?> SignIn(ExternalSignInRequest model);

        Task<ApplicationUser?> CreateUser(CreateUserRequest model);
    }
}