using Ratbags.Accounts.API.Models.Accounts;
using Ratbags.Accounts.API.Models.API.Tokens;

namespace Ratbags.Accounts.Interfaces;

public interface ILoginService
{
    Task<RefreshTokenAndJWTOrchestratorResponse?> Login(LoginModel model);
}