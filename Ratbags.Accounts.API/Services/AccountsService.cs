using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Ratbags.Accounts.API.Interfaces;
using Ratbags.Accounts.API.Models.DB;

namespace Ratbags.Accounts.API.Services;

public class AccountsService : IAccountsService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<AccountsService> _logger;

    public AccountsService(
        UserManager<ApplicationUser> userManager,
        ILogger<AccountsService> logger)
    {
        _userManager = userManager;
        _logger = logger;
    }

    public async Task<(string?, string?)> GetUserNameDetails(Guid id)
    {
        var user = await _userManager.FindByIdAsync(id.ToString());

        if (user == null)
        {
            return (null, null);
        }

        return (user.FirstName, user.LastName);
    }

    public async Task<Dictionary<Guid, string>?> GetUserNameDetails(IReadOnlyList<Guid> ids)
    {
        var results = new Dictionary<Guid, string>();

        var idStrings = ids.Select(x => x.ToString()).ToList();
        
        foreach(var id in ids)
        {
            var (firstname, lastname) = await GetUserNameDetails(id);

            if (!results.ContainsKey(id))
            {
                results.Add(id, $"{firstname ?? ""} {lastname ?? ""}".Trim());
            }
        }

        return results;
    }
}