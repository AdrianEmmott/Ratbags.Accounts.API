using Ratbags.Core.DTOs.Articles;

namespace Ratbags.Accounts.API.Interfaces;

public interface IMassTransitService
{
    Task SendRegisterConfirmEmailRequest(string name, string email, Guid userId, string token);
    Task SendForgotPasswordEmailRequest(string name, string email, Guid userId, string token);
}
