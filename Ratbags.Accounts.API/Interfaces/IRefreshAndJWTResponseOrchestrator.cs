using Ratbags.Accounts.API.Models.API;
using Ratbags.Accounts.API.Models.DB;

namespace Ratbags.Accounts.Interfaces;

public interface IRefreshAndJWTResponseOrchestrator
{
    Task<RefreshTokenAndJWTResponse?> CreateResponseAsync(ApplicationUser user,
        string? existingCookie = null);
}