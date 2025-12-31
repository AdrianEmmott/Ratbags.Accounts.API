using Microsoft.AspNetCore.Identity;
using Ratbags.Accounts.API.Interfaces;
using Ratbags.Accounts.API.Models.DB;
using Ratbags.Accounts.API.Models.ResetPassword;
using Ratbags.Accounts.Interfaces;
using System.Web;

namespace Ratbags.Accounts.API.Services;

public class ResetPasswordService : IResetPasswordService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<ResetPasswordService> _logger;
    private readonly IAccountsServiceBusService _accountsServiceBusService;

    public ResetPasswordService(
        UserManager<ApplicationUser> userManager,
        ILogger<ResetPasswordService> logger,
        IAccountsServiceBusService accountsServiceBusService)
    {
        _userManager = userManager;
        _logger = logger;
        _accountsServiceBusService = accountsServiceBusService;
    }

    public async Task<bool> ResetRequest(PasswordResetRequest model)
    {
        var user = await _userManager.FindByEmailAsync(model.Email);

        if (user?.Email?.Length > 0)
        {
            // stop VS whining about nullable users
            var nonNullUser = user; 

            // create password reset token
            var resetToken = await _userManager
                .GeneratePasswordResetTokenAsync(nonNullUser);

            var encodedToken = HttpUtility.UrlEncode(resetToken);

            var sendEmailSuccess = await _accountsServiceBusService.SendForgotPasswordEmailRequestAsync(
                name: nonNullUser.FirstName ?? string.Empty,
                email: nonNullUser.Email,
                userId: Guid.Parse(nonNullUser.Id),
                token: encodedToken);

            return sendEmailSuccess;
        }
        else
        {
            _logger.LogWarning($"user {model.Email} requested a password reset - user does not exist");
            
            return false;
        }
    }

    public async Task<bool> Update(PasswordResetUpdate model)
    {
        var user = await _userManager.FindByIdAsync(model.UserId.ToString());

        if (user == null)
        {
            _logger.LogWarning($"user {model.UserId} requested a password reset - user does not exist");

            return false;
        }

        var result = await _userManager
            .ResetPasswordAsync(
                user,
                model.PasswordResetToken.ToString(),
                model.Password
             );

        if (result.Succeeded)
        {
            _logger.LogInformation($"password reset for user {user.Email}");
            return result.Succeeded;
        }

        foreach (var error in result.Errors.Select((error, index) => (error, index)))
        {
            _logger.LogWarning(@$"user {user.Email} attempted to reset their pasword 
                                but failed: {error.index} 
                                of {result.Errors.Count()} {error.error.Description}");
        }

        return false;
    }
}