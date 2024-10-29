using Ratbags.Accounts.API.Models.API;

namespace Ratbags.Accounts.API.Interfaces;

public interface IRefreshTokenService
{
    Task<string?> CreateAsync(Guid userId);
    Task<bool> RevokeAsync(Guid userId, bool all = false);
    Task<bool> ValidateAsync(string token);
}