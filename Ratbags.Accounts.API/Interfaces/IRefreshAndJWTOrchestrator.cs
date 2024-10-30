using Ratbags.Accounts.API.Models.API.Tokens;

namespace Ratbags.Accounts.Interfaces;

public interface IRefreshAndJWTOrchestrator
{
    Task<RefreshTokenAndJWTOrchestratorResponse?> CreateResponseAsync(RefreshTokenAndJWTOrchestratorRequest model);
}