using MassTransit;
using Ratbags.Core.Settings;

namespace Ratbags.Emails.API.ServiceExtensions;

public static class MassTransitServiceExtension
{
    public static IServiceCollection AddMassTransitWithRabbitMqServiceExtension(
        this IServiceCollection services, 
        AppSettingsBase appSettings)
    {
        services.AddMassTransit(x =>
        {
            x.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host($"rabbitmq://{appSettings.Messaging.Hostname}/{appSettings.Messaging.VirtualHost}", h =>
                {
                    h.Username(appSettings.Messaging.Username);
                    h.Password(appSettings.Messaging.Password);
                });
            });
        });

        return services;
    }
}