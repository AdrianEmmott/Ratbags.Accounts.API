﻿using MassTransit;
using Ratbags.Accounts.API.Messaging.Consumers;
using Ratbags.Accounts.API.Models;
using Ratbags.Core.Events.Accounts;

namespace Ratbags.Emails.API.ServiceExtensions;

public static class MassTransitServiceExtension
{
    public static IServiceCollection AddMassTransitWithRabbitMqServiceExtension(
        this IServiceCollection services, 
        AppSettings appSettings)
    {
        services.AddMassTransit(x =>
        {
            x.AddConsumer<UserNameDetailsConsumer>();

            x.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host($"rabbitmq://{appSettings.Messaging.Hostname}/{appSettings.Messaging.VirtualHost}", h =>
                {
                    h.Username(appSettings.Messaging.Username);
                    h.Password(appSettings.Messaging.Password);
                });

                cfg.Message<UserFullNameResponse>(c =>
                {
                    c.SetEntityName("accounts.user-full-name"); // exchange name for message type
                });
                
                cfg.ReceiveEndpoint("accounts.user-full-name", e =>
                {
                    e.ConfigureConsumer<UserNameDetailsConsumer>(context);
                });
            });
        });

        return services;
    }
}