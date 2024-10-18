using MassTransit;
using Ratbags.Accounts.API.Interfaces;
using Ratbags.Core.Events.Accounts;

namespace Ratbags.Accounts.API.Services;

public class MassTransitService : IMassTransitService
{
    private readonly ISendEndpointProvider _sendEndpointProvider;

    public MassTransitService(
        ISendEndpointProvider sendEndpointProvider)
    {
        _sendEndpointProvider = sendEndpointProvider;
    }

    public async Task SendRegisterConfirmEmailRequest(string name, string email, Guid userId, string token)
    {
        var endpoint = await _sendEndpointProvider.GetSendEndpoint(new Uri("queue:accounts.register.confirm-email"));
        await endpoint.Send(new SendRegisterConfirmEmailRequest
        {
            Name = name,
            Email = email,
            UserId = userId,
            Token = token
        });
    }

    public async Task SendForgotPasswordEmailRequest(string name, string email, Guid userId, string token)
    {
        var endpoint = await _sendEndpointProvider.GetSendEndpoint(new Uri("queue:accounts.forgot-password.email"));
        await endpoint.Send(new SendForgotPasswordEmailRequest
        {
            Name = name,
            Email = email,
            UserId = userId,
            Token = token
        });
    }
}
