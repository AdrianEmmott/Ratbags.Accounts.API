using Ratbags.Accounts.API.Models.API;
using Ratbags.Accounts.API.Models.DB;

namespace Ratbags.Accounts.API.Interfaces;

public interface IRefreshTokenRepository
{
    Task<bool> CreateAsync(RefreshTokenCreate model);
    Task<string?> GetAsync(Guid userId);
    Task<bool> RevokeAsync(Guid userId, bool all = false);
    Task<bool> ValidateAsync(string token);
}