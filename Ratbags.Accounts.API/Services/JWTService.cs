using Microsoft.IdentityModel.Tokens;
using Ratbags.Accounts.API.Models;
using Ratbags.Accounts.API.Models.DB;
using Ratbags.Accounts.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Ratbags.Accounts.Services;

public class JWTService : IJWTService
{
    private readonly AppSettings _appSettings;

    public JWTService(AppSettings appSettings)
    {
        _appSettings = appSettings;
    }

    /// <summary>
    /// Generates JWT token when signing in with email password
    /// </summary>
    /// <param name="user">The user signing in</param>
    /// <returns>JWT</returns>
    public string GenerateJwtToken(ApplicationUser user)
    {
        // grab claims
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id),
            new Claim(JwtRegisteredClaimNames.Email, user.Email ?? ""),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_appSettings.JWT.Secret));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        // create token
        var token = new JwtSecurityToken(
            issuer: _appSettings.JWT.Issuer,
            audience: _appSettings.JWT.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_appSettings.Tokens.JWT.ExpiryAddMinutes),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}