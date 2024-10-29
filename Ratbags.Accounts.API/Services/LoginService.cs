using Microsoft.AspNetCore.Identity;
using Ratbags.Accounts.API.Models.API;
using Ratbags.Accounts.API.Models.DB;
using Ratbags.Accounts.Interfaces;
using Ratbags.Core.Models.Accounts;

namespace Ratbags.Accounts.Services;

public class LoginService : ILoginService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IRefreshAndJWTResponseOrchestrator _refreshAndJWTResponseOrchestrator;
    private readonly ILogger<LoginService> _logger;

    public LoginService(UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        IRefreshAndJWTResponseOrchestrator refreshAndJWTResponseOrchestrator,
        ILogger<LoginService> logger)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _refreshAndJWTResponseOrchestrator = refreshAndJWTResponseOrchestrator;
        _logger = logger;
    }

    public async Task<RefreshTokenAndJWTResponse?> Login(LoginModel model)
    {
        var user = await _userManager.FindByEmailAsync(model.Email);

        if (user != null && user.EmailConfirmed)
        {
            var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);

            if (result.Succeeded)
            {
                var response = await _refreshAndJWTResponseOrchestrator
                    .CreateResponseAsync(user);

                return response;
            }
        }

        return null;
    }
}