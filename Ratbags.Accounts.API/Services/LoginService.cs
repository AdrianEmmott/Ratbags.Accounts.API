using Microsoft.AspNetCore.Identity;
using Ratbags.Accounts.API.Models.Accounts;
using Ratbags.Accounts.API.Models.API.Tokens;
using Ratbags.Accounts.API.Models.DB;
using Ratbags.Accounts.Interfaces;

namespace Ratbags.Accounts.Services;

public class LoginService : ILoginService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IRefreshAndJWTOrchestrator _refreshAndJWTResponseOrchestrator;
    private readonly ILogger<LoginService> _logger;

    public LoginService(UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        IRefreshAndJWTOrchestrator refreshAndJWTResponseOrchestrator,
        ILogger<LoginService> logger)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _refreshAndJWTResponseOrchestrator = refreshAndJWTResponseOrchestrator;
        _logger = logger;
    }

    public async Task<RefreshTokenAndJWTOrchestratorResponse?> Login(LoginModel model)
    {
        var user = await _userManager.FindByEmailAsync(model.Email);

        if (user?.EmailConfirmed == true)
        {
            var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);

            if (result.Succeeded)
            {
                var response = await _refreshAndJWTResponseOrchestrator
                    .CreateResponseAsync(new RefreshTokenAndJWTOrchestratorRequest { User = user });

                return response;
            }
        }

        return null;
    }
}