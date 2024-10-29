using Ratbags.Accounts.API.Models.API;
using Ratbags.Core.Models.Accounts;

namespace Ratbags.Accounts.Interfaces;

public interface ILoginService
{
    Task<RefreshTokenAndJWTResponse?> Login(LoginModel model);
}