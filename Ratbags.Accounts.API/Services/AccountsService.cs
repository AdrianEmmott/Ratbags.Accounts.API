using Microsoft.AspNetCore.Identity;
using Ratbags.Accounts.API.Interfaces;
using Ratbags.Accounts.API.Models.DB;

namespace Ratbags.Accounts.Services;

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

    public async Task<string?> GetUserNameDetails(Guid id)
    {
        var user = await _userManager.FindByIdAsync(id.ToString());

        if (user == null)
        {
            return null;
        }

        return $"{user.FirstName} {user.LastName}";
    }
}