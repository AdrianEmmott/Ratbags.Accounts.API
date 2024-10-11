using Microsoft.IdentityModel.Tokens;
using Ratbags.Account.Interfaces;
using Ratbags.Account.Models;
using Ratbags.Core.Settings;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Ratbags.Account.Services;

public class JWTService : IJWTService
{
    private readonly AppSettingsBase _appSettings;

    public JWTService(AppSettingsBase appSettings)
    {
        _appSettings = appSettings;
    }

    /// <summary>
    /// Generats JWT token when signing in with email password
    /// </summary>
    /// <param name="user">The user signing in</param>
    /// <returns>JWT Token</returns>
    public string GenerateJwtToken(ApplicationUser user)
    {
        // grab claims
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        //var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Secret"]));
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_appSettings.JWT.Secret));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        // create token
        var token = new JwtSecurityToken(
            issuer: _appSettings.JWT.Issuer,
            audience: _appSettings.JWT.Audience,
            claims,
            expires: DateTime.Now.AddMinutes(30),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    /// <summary>
    /// Generats JWT token when signing in via external provider - Google, Facebook etc
    /// </summary>
    /// <param name="claims">Claims handed over by provider</param>
    /// <returns>JWT Token</returns>
    public string GenerateJwtToken(IEnumerable<Claim> claims)
    {
        // create key
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_appSettings.JWT.Secret));

        // create signing credentials
        var credential = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        // create token
        var token = new JwtSecurityToken(
            issuer: _appSettings.JWT.Issuer,
            audience: _appSettings.JWT.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(30),
            signingCredentials: credential);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}