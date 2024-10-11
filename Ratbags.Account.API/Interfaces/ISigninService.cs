using Microsoft.AspNetCore.Authentication;
using Ratbags.Account.API.Models;

namespace Ratbags.Account.API.Interfaces
{
    public interface ISigninService
    {
        Task CreateUser(AuthenticateResult authenticateResult, string providerName);

        Task<TokenResult?> CreateToken(AuthenticateResult authenticateResult);
    }
}