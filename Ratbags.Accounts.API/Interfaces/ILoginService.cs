using Microsoft.AspNetCore.Identity.Data;
using Ratbags.Accounts.API.Models.API.Tokens;

namespace Ratbags.Accounts.Interfaces;

public interface ILoginService
{
    Task<RefreshTokenAndJWTOrchestratorResponse?> Login(LoginRequest model);
}