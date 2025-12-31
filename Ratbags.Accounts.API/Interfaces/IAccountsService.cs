namespace Ratbags.Accounts.API.Interfaces;

public interface IAccountsService
{
    Task<(string?, string?)> GetUserNameDetails(Guid id);

    Task<Dictionary<Guid, string>?> GetUserNameDetails(IReadOnlyList<Guid> ids);
}