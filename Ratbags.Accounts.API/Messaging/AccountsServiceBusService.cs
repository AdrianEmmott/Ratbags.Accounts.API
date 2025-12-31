using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Options;
using Ratbags.Accounts.API.Interfaces;
using Ratbags.Accounts.API.Models;
using Ratbags.Core.Messaging.ASB.RequestReponse;
using System.Text.Json;

namespace Ratbags.Accounts.API.Messaging;

public class AccountsServiceBusService : ServiceBusService<AccountsServiceBusService>, IAccountsServiceBusService
{
    private readonly ILogger<AccountsServiceBusService> _logger;
    private readonly AppSettings _appSettings;
    
    public AccountsServiceBusService(
        AppSettings appSettings,
        ServiceBusClient sbClient,
        ILogger<AccountsServiceBusService> logger,
        IOptions<JsonSerializerOptions> jsonOptions)
        : base(
            sbClient,
            logger,
            jsonOptions,
            appSettings.Messaging.ASB.ResponseTopic,
            appSettings.Messaging.ASB.ResponseSubscription)
    {
        _logger = logger;
        _appSettings = appSettings;
    }

    public async Task<bool> SendForgotPasswordEmailRequestAsync(
        string name, 
        string email, 
        Guid userId, 
        string token)
    {
        try
        {
            var requestTopic = _appSettings.MessagingExtensions.ForgotPasswordEmailRequestTopic;

            var request = new ForgotPasswordEmailRequest(name, email, userId, token);

            _logger.LogInformation(
                "Sending forgot password email request for user {userId} to {Topic} topic", requestTopic, userId);

            var response = await SendRequestAsync<ForgotPasswordEmailRequest, ForgotPasswordEmailResponse>(request, requestTopic);

            return response?.success ?? false;
        }
        catch (Exception e)
        {
            _logger.LogError($"Bus error sending forgot password email request for user {userId}: {e.Message}");
            throw;
        }
    }
}