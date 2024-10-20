namespace Ratbags.Accounts.API.Interfaces;

public interface IAccountsService
{
    Task<string?> GetUserNameDetails(Guid id);
}