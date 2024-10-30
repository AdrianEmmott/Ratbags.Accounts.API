using Microsoft.AspNetCore.Identity;
using Ratbags.Accounts.API.Interfaces;
using Ratbags.Accounts.API.Models;
using Ratbags.Accounts.API.Models.DB;
using Ratbags.Accounts.Interfaces;
using Ratbags.Accounts.Models.API.Register;
using System.Web;

namespace Ratbags.Accounts.Services;

public class RegisterService: IRegisterService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IMassTransitService _massTransitService;
    private readonly ILogger<RegisterService> _logger;

    public RegisterService(
        UserManager<ApplicationUser> userManager,
        IMassTransitService massTransitService,
        ILogger<RegisterService> logger)
    {
        _userManager = userManager;
        _massTransitService = massTransitService;
        _logger = logger;
    }

    public async Task<bool> Register(RegisterModel model)
    {
        var user = new ApplicationUser
        {
            UserName = model.Email,
            Email = model.Email,
            FirstName = model.FirstName,
            LastName = model.LastName,
        };

        // stop VS whining about nullable users
        var nonNullUser = user;

        var result = await _userManager.CreateAsync(user, model.Password);

        if (result.Succeeded && user?.Email?.Length > 0)
        {
            _logger.LogInformation($"registered user {model.Email}: {nonNullUser.Id}");

            // create confirm email token
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(nonNullUser);
            var encodedToken = HttpUtility.UrlEncode(token);

            await _massTransitService.SendRegisterConfirmEmailRequest(
                nonNullUser.FirstName ?? string.Empty, 
                nonNullUser.Email, 
                Guid.Parse(nonNullUser.Id), 
                encodedToken);

            return true;
        }

        foreach (var error in result.Errors.Select((error, index) => (error, index)))
        {
            _logger.LogWarning(@$"user {nonNullUser.Email} attempted to confirm their pasword 
                                but failed: {error.index} 
                                of {result.Errors.Count()} {error.error.Description}");
        }

        return false;
    }

    public async Task<bool> RegisterComfirm(RegisterConfirmEmailModel model)
    {
        var user = await _userManager.FindByIdAsync(model.UserId.ToString());

        if (user == null)
        {
            _logger.LogWarning($"user {model.UserId} attempted to confirm their email address but user not found");
            return false;
        }

        var result = await _userManager.ConfirmEmailAsync(user, model.Token);

        if (result.Succeeded)
        {
            _logger.LogInformation($"email confirmed for user {user.Email}");
            return true;
        }

        foreach (var error in result.Errors.Select((error, index) => (error, index)))
        {
            _logger.LogWarning(@$"user {user.Email} attempted to confirm their pasword 
                                but failed: {error.index} 
                                of {result.Errors.Count()} {error.error.Description}");
        }

        return false;
    }

    public async Task<bool> ResendConfirmationEmail(ResendEmailConfirmationModel model)
    {
        var user = await _userManager.FindByEmailAsync(model.Email);

        if (user != null)
        {
            // create confirm email token
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var encodedToken = HttpUtility.UrlEncode(token);
            var confirmUrl = $"https://localhost:4200/register-confirm-email/{user.Id}/{encodedToken}";

            _logger.LogInformation($"sent register comfirl email for {user.Email}");

            return true;
        }

        return false;
    }
}