using Ratbags.Core.DTOs.Articles;

namespace Ratbags.Accounts.API.Interfaces
{
    public interface IAccountsServiceBusService
    {
        Task<bool> SendForgotPasswordEmailRequestAsync(string name, string email, Guid userId, string token);
    }
}