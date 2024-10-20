using MassTransit;
using Ratbags.Accounts.API.Interfaces;
using Ratbags.Core.Events.Accounts;

namespace Ratbags.Accounts.API.Messaging.Consumers;

public class UserNameDetailsConsumer : IConsumer<UserFullNameRequest>
{
    private readonly ILogger<UserNameDetailsConsumer> _logger;
    private readonly IAccountsService _accountsService;

    public UserNameDetailsConsumer(
        IAccountsService accountsService,
        ILogger<UserNameDetailsConsumer> logger)
    {
        _accountsService = accountsService;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<UserFullNameRequest> context)
    {
        _logger.LogInformation("**************************** listening...");

        var name = await _accountsService.GetUserNameDetails(context.Message.UserId);

        _logger.LogInformation($"getting name details for user {context.Message.UserId}");

        // respond to the request
        await context.RespondAsync(new UserFullNameResponse
        {
            FullName = name ?? string.Empty,
        });
    }
}
