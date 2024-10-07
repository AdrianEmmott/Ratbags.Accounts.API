using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Ratbags.Account.Models;
using Ratbags.Core.Settings;
using Ratbags.Login.Models;
using System.Text;

namespace Ratbags.Account.ServiceExtensions;

public static class AuthenticationServiceExtension
{
    public static IServiceCollection AddAuthenticationExtension(this IServiceCollection services, AppSettingsBase settings)
    {
        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = settings.JWT.Issuer,
                ValidAudience = settings.JWT.AudienceType,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(settings.JWT.Secret))
            };
        })
        .AddGoogle(options =>
        {
            options.ClientId = "your-google-client-id";
            options.ClientSecret = "your-google-client-secret";
        })
        .AddFacebook(options =>
        {
            options.AppId = "your-facebook-app-id";
            options.AppSecret = "your-facebook-app-secret";
        });

        return services;
    }
}
