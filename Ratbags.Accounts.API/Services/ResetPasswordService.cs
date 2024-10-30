using Microsoft.AspNetCore.Identity;
using Ratbags.Accounts.API.Interfaces;
using Ratbags.Accounts.API.Models.DB;
using Ratbags.Accounts.API.Models.ResetPassword;
using Ratbags.Accounts.Interfaces;
using System.Web;

namespace Ratbags.Accounts.Services;

public class ResetPasswordService : IResetPasswordService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IMassTransitService _massTransitService;
    private readonly ILogger<ResetPasswordService> _logger;

    public ResetPasswordService(
        UserManager<ApplicationUser> userManager,
        IMassTransitService massTransitService,
        ILogger<ResetPasswordService> logger)
    {
        _userManager = userManager;
        _massTransitService = massTransitService;
        _logger = logger;
    }

    public async Task<bool> ResetRequest(PasswordResetRequestModel model)
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

            await _massTransitService.SendForgotPasswordEmailRequest(
                nonNullUser.FirstName ?? string.Empty,
                nonNullUser.Email,
                Guid.Parse(nonNullUser.Id),
                encodedToken);

            return true;
        }
        else
        {
            _logger.LogWarning($"user {model.Email} requested a password reset - user does not exist");
            
            return false;
        }
    }

    public async Task<bool> Update(PasswordResetUpdateModel model)
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