using Ratbags.Accounts.API.Interfaces;
using Ratbags.Accounts.API.Models.API;
using Ratbags.Accounts.API.Models.DB;
using Ratbags.Accounts.Interfaces;
using System.Net;

namespace Ratbags.Accounts.Services;

public class RefreshAndJWTResponseOrchestrator : IRefreshAndJWTResponseOrchestrator
{
    private readonly IJWTService _jwtService;
    private readonly IRefreshTokenService _refreshTokenService;
    private readonly ILogger<RefreshAndJWTResponseOrchestrator> _logger;

    public RefreshAndJWTResponseOrchestrator(
        IJWTService jWTService,
        IRefreshTokenService refreshTokensService,
        ILogger<RefreshAndJWTResponseOrchestrator> logger)
    {
        _jwtService = jWTService;
        _refreshTokenService = refreshTokensService;
        _logger = logger;
    }

    public async Task<RefreshTokenAndJWTResponse?> CreateResponseAsync(
        ApplicationUser user, 
        string? existingCookie = null)
    {
        var refreshToken = "";

        
        if (existingCookie != null)
        {
            // we may have an existing cookie if we're coming in here from
            // refresh token endpoint
            var validToken = await _refreshTokenService
                .ValidateAsync(existingCookie);

            if (!validToken)
            {
                _logger.LogWarning($"invalid existing token");
                return null;
            }

            // if token is valid, we still request a new one
            // so we have a rolling expiry date
        }

        // login / external sign-in
        refreshToken = await _refreshTokenService
                    .CreateAsync(Guid.Parse(user.Id));
        
        // revoke all previous refresh tokens for user
        if (refreshToken != null)
        {
            await _refreshTokenService.RevokeAsync(Guid.Parse(user.Id));
        }

        // create jwt
        var jwt = _jwtService.GenerateJwtToken(user);


        // return refresh token and jwt
        if (refreshToken != null && jwt != null)
        {
            var response = new RefreshTokenAndJWTResponse
            {
                RefreshToken = refreshToken,
                JWT = jwt
            };

            return response;
        }

        return null;
    }
}