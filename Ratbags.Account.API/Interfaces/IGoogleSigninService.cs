using Microsoft.AspNetCore.Authentication;
using Ratbags.Account.API.Models;

namespace Ratbags.Account.API.Interfaces
{
    public interface IGoogleSigninService
    {
        Task CreateUser(AuthenticateResult authenticateResult);

        Task<TokenResult?> CreateToken(AuthenticateResult authenticateResult);
    }
}