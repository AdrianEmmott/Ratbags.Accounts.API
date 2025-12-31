using Ratbags.Accounts.API.Interfaces;
using Ratbags.Core.Messaging.ASB.RequestReponse;

namespace Ratbags.Accounts.API.Messaging;

public sealed class GetUserNameDetailsHandler
    : IServiceBusRequestHandler<GetUserNameDetailsRequest, GetUserNameDetailsResponse?>
{
    private readonly IAccountsService _service;

    public GetUserNameDetailsHandler(IAccountsService service)
    {
        _service = service;
    }

    public async Task<GetUserNameDetailsResponse?> HandleAsync(
        GetUserNameDetailsRequest request, 
        CancellationToken ct)
    {
        var usersNameDetails = await _service.GetUserNameDetails(request.userIds);

        return new GetUserNameDetailsResponse(userNameDetails: usersNameDetails);
    }
}
