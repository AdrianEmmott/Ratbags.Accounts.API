using Microsoft.AspNetCore.Identity;
using Ratbags.Accounts.API.Interfaces;
using Ratbags.Accounts.API.Models;
using Ratbags.Accounts.API.Models.API;
using Ratbags.Accounts.API.Models.DB;
using System.Security.Cryptography;

namespace Ratbags.Accounts.Services;

public class RefreshTokenService : IRefreshTokenService
{
    private readonly IRefreshTokenRepository _refreshTokensRepository;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly AppSettings _appSettings;
    private readonly ILogger<RefreshTokenService> _logger;

    public RefreshTokenService(
        IRefreshTokenRepository refreshTokensRepository,
        UserManager<ApplicationUser> userManager,
        AppSettings appSettings,
        ILogger<RefreshTokenService> logger)
    {
        _refreshTokensRepository = refreshTokensRepository;
        _userManager = userManager;
        _appSettings = appSettings;
        _logger = logger;
    }

    /// <summary>
    /// Creates a new refresh token. Typically called by RequestAsync but 
    /// can be called independently by other services - e.g. LoginService
    /// </summary>
    /// <param name="userId">
    /// Id of user requesting refresh token
    /// </param>
    /// <returns>
    /// Refresh token value
    /// </returns>
    /// <remarks>
    /// If calling from another service/controller, also call
    /// RevokeAsync to invalidate previous refresh tokens
    /// </remarks>
    public async Task<string?> CreateAsync(Guid userId)
    {
        var model = new RefreshTokenCreate
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Created = DateTime.UtcNow,
            Token = GenerateRefreshToken(),
            Expires = DateTime.UtcNow.AddMinutes(_appSettings.Tokens.RefreshToken.ExpiryAddMinutes),
        };

        var result = await _refreshTokensRepository.CreateAsync(model);

        return model.Token;
    }

    public async Task<bool> RevokeAsync(Guid userId, bool all = false)
    {
        var result = await _refreshTokensRepository.RevokeAsync(userId);

        return result;
    }

    public async Task<bool> ValidateAsync(string token)
    {
        var result = await _refreshTokensRepository.ValidateAsync(token);

        return result; 
    }

    // private
    private string GenerateRefreshToken()
    {
        var randomNumber = new byte[64];

        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }
    }

    private async Task<string?> GetAsync(Guid userId)
    {
        var result = await _refreshTokensRepository.GetAsync(userId);

        return result;
    }
}