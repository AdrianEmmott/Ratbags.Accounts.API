using Microsoft.Extensions.Options;
using Ratbags.Accounts.API.Interfaces;
using Ratbags.Accounts.API.Models;
using Ratbags.Accounts.API.Repositories;
using Ratbags.Accounts.API.Services;
using Ratbags.Accounts.Interfaces;
using Ratbags.Accounts.Services;

namespace Ratbags.Accounts.ServiceExtensions;

public static class DIServiceExtension
{
    public static IServiceCollection AddDIServiceExtension(this IServiceCollection services)
    {
        services.AddScoped<IAccountsService, AccountsService>();
        services.AddScoped<IRegisterService, RegisterService>();
        services.AddScoped<ILoginService, LoginService>();
        services.AddScoped<IResetPasswordService, ResetPasswordService>();

        services.AddScoped<IExternalSigninService, ExternalSigninService>();
        
        services.AddScoped<IJWTService, JWTService>();
        
        services.AddScoped<IRefreshTokenService, RefreshTokenService>();
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
        services.AddScoped<IRefreshAndJWTOrchestrator, RefreshAndJWTOrchestrator>();

        services.AddScoped<IMassTransitService, MassTransitService>();

        // expose appSettings base as IOptions<T> singleton
        services.AddSingleton(x => x.GetRequiredService<IOptions<AppSettings>>().Value);

        return services;
    }
}
