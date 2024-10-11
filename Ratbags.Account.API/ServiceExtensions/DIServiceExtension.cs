﻿using Microsoft.Extensions.Options;
using Ratbags.Account.API.Interfaces;
using Ratbags.Account.Interfaces;
using Ratbags.Account.Services;
using Ratbags.Core.Settings;

namespace Ratbags.Account.ServiceExtensions;

public static class DIServiceExtension
{
    public static IServiceCollection AddDIServiceExtension(this IServiceCollection services)
    {
        services.AddScoped<IJWTService, JWTService>();
        services.AddScoped<ILoginService, LoginService>();
        services.AddScoped<IExternalSigninService, ExternalSigninService>();

        // expose appSettings base as IOptions<T> singleton
        services.AddSingleton(x => x.GetRequiredService<IOptions<AppSettingsBase>>().Value);

        return services;
    }
}
