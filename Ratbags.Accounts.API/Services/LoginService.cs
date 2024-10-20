using Microsoft.AspNetCore.Identity;
using Ratbags.Account.API.Models;
using Ratbags.Account.Interfaces;
using Ratbags.Accounts.API.Models.DB;
using Ratbags.Core.Models.Accounts;

namespace Ratbags.Account.Services;

public class LoginService : ILoginService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IJWTService _jwtService;
    private readonly ILogger<LoginService> _logger;

    public LoginService(UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        IJWTService jWTService,
        ILogger<LoginService> logger)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _jwtService = jWTService;
        _logger = logger;
    }

    public async Task<TokenResult?> Login(LoginModel model)
    {
        var user = await _userManager.FindByEmailAsync(model.Email);

        if (user != null && user.EmailConfirmed)
        {
            var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);

            if (result.Succeeded)
            {
                var token = new TokenResult { Token = _jwtService.GenerateJwtToken(user), Email = model.Email };
                return token;
            }
        }

        return null;
    }
}