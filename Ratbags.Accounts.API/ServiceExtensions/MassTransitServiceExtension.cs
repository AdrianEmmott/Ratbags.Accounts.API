using MassTransit;
using Ratbags.Core.Events.Accounts;
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

                //cfg.Message<SendRegisterConfirmEmailRequest>(c =>
                //{
                //    c.SetEntityName("accounts.register.confirm-email"); // set exchange name for this message type
                //});

                //cfg.Message<SendForgotPasswordEmailRequest>(c =>
                //{
                //    c.SetEntityName("accounts.forgot-password.email"); // set exchange name for this message type
                //});
            });
        });

        return services;
    }
}