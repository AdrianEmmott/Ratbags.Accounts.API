using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Ratbags.Core.Settings;
using System.Text;

namespace Ratbags.Account.ServiceExtensions;

public static class AuthenticationServiceExtension
{
    public static IServiceCollection AddAuthenticationServiceExtension(this IServiceCollection services, AppSettingsBase settings)
    {
        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
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
                ValidAudience = settings.JWT.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(settings.JWT.Secret))
            };
        })
        .AddCookie()
        .AddGoogle(options =>
        {
            options.ClientId = settings.ExternalAuthentication.Google.ClientId;
            options.ClientSecret = settings.ExternalAuthentication.Google.ClientSecret;
            options.CallbackPath = new PathString("/signin-google"); // route must exist in ocelot

            // force consent screen to show
            options.Events.OnRedirectToAuthorizationEndpoint = context =>
            {
                context.Response.Redirect(context.RedirectUri + "&prompt=consent");
                return Task.CompletedTask;
            };
        })
        .AddFacebook(options =>
        {
            options.ClientId = settings.ExternalAuthentication.Facebook.ClientId;
            options.ClientSecret = settings.ExternalAuthentication.Facebook.ClientSecret;
            options.CallbackPath = new PathString("/signin-facebook"); // route must exist in ocelot
        });

        return services;
    }
}