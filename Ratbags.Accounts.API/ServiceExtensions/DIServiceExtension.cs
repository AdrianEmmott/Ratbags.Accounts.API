using Microsoft.Extensions.Options;
using Ratbags.Accounts.API.Interfaces;
using Ratbags.Accounts.API.Repositories;
using Ratbags.Accounts.API.Services;
using Ratbags.Accounts.Interfaces;
using Ratbags.Accounts.Services;
using Ratbags.Core.Settings;

namespace Ratbags.Accounts.ServiceExtensions;

public static class DIServiceExtension
{
    public static IServiceCollection AddDIServiceExtension(this IServiceCollection services)
    {
        services.AddScoped<ILoginService, LoginService>();
        services.AddScoped<IExternalSigninService, ExternalSigninService>();
        services.AddScoped<IAccountsService, AccountsService>();

        services.AddScoped<IJWTService, JWTService>();
        

        services.AddScoped<IRefreshTokenService, RefreshTokenService>();
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
        services.AddScoped<IRefreshAndJWTResponseOrchestrator, RefreshAndJWTResponseOrchestrator>();

        services.AddScoped<IMassTransitService, MassTransitService>();

        // expose appSettings base as IOptions<T> singleton
        services.AddSingleton(x => x.GetRequiredService<IOptions<AppSettingsBase>>().Value);

        return services;
    }
}
