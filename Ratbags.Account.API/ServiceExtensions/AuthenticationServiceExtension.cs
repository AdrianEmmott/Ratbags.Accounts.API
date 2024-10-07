using Ratbags.Core.Settings;

namespace Ratbags.Account.ServiceExtensions;

public static class AuthenticationServiceExtension
{
    public static IServiceCollection AddAuthentication(this IServiceCollection services, AppSettingsBase settings)
    {
        return services;
    }

}
