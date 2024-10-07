using Ratbags.Account.Interfaces;
using Ratbags.Account.Services;

namespace Ratbags.Account.ServiceExtensions;

public static class DIServiceExtension
{
    public static IServiceCollection AddDIServiceExtension(this IServiceCollection services)
    {
        services.AddScoped<IJWTService, JWTService>();
        services.AddScoped<ILoginService, LoginService>();

        return services;
    }
}
