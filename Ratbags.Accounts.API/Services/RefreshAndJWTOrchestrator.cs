using Ratbags.Accounts.API.Interfaces;
using Ratbags.Accounts.API.Models.API.Tokens;
using Ratbags.Accounts.Interfaces;

namespace Ratbags.Accounts.Services;

/// <summary>
/// 
/// </summary>
public class RefreshAndJWTOrchestrator : IRefreshAndJWTOrchestrator
{
    private readonly IJWTService _jwtService;
    private readonly IRefreshTokenService _refreshTokenService;
    private readonly ILogger<RefreshAndJWTOrchestrator> _logger;

    public RefreshAndJWTOrchestrator(
        IJWTService jWTService,
        IRefreshTokenService refreshTokensService,
        ILogger<RefreshAndJWTOrchestrator> logger)
    {
        _jwtService = jWTService;
        _refreshTokenService = refreshTokensService;
        _logger = logger;
    }

    public async Task<RefreshTokenAndJWTOrchestratorResponse?> CreateResponseAsync(RefreshTokenAndJWTOrchestratorRequest model)
    {
        if (model.ExistingRefreshToken != null)
        {
            // we may have an existing cookie if we're coming in here from
            // refresh token endpoint
            var validToken = await _refreshTokenService
                .ValidateAsync(model.ExistingRefreshToken);

            if (!validToken)
            {
                _logger.LogWarning($"invalid existing token");
                return null;
            }

            // if token is valid, still request a new one
            // so we have a rolling expiry date
        }

        // login / external sign-in
        var refreshToken = await _refreshTokenService
                    .CreateAsync(Guid.Parse(model.User.Id));
        
        // revoke all previous refresh tokens for user
        if (refreshToken != null)
        {
            await _refreshTokenService.RevokeAsync(Guid.Parse(model.User.Id));
        }

        // create jwt
        var jwt = _jwtService.GenerateJwtToken(model.User);


        // return refresh token and jwt
        if (refreshToken != null && jwt != null)
        {
            var response = new RefreshTokenAndJWTOrchestratorResponse
            {
                RefreshToken = refreshToken,
                JWT = jwt
            };

            return response;
        }

        return null;
    }
}