using Ratbags.Accounts.API.Models;
using Ratbags.Core.Messaging.ASB.RequestReponse;

namespace Ratbags.Accounts.API.Messaging;

public sealed class GetUserNameDetailsWorker : 
    ServiceBusRequestReplyWorker<GetUserNameDetailsRequest, GetUserNameDetailsResponse>
{
    public GetUserNameDetailsWorker(
        AppSettings appSettings,
        IServiceScopeFactory scopeFactory,
        ILogger<GetUserNameDetailsWorker> logger)
        : base(
            appSettings.Messaging.ASB.Connection,
            appSettings.MessagingExtensions.UserNameDetailsRequestTopic,
            appSettings.Messaging.ASB.ResponseSubscription,
            scopeFactory,
            logger)
    { }
}