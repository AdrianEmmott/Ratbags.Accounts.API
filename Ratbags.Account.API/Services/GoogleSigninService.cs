using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Ratbags.Account.API.Interfaces;
using Ratbags.Account.API.Models;
using Ratbags.Account.Interfaces;
using Ratbags.Account.Models;
using System.Security.Claims;

namespace Ratbags.Account.Services
{
    public class GoogleSigninService : IGoogleSigninService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IJWTService _jwtService;
        private readonly ILogger<GoogleSigninService> _logger;

        public GoogleSigninService(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IJWTService jWTService,
            ILogger<GoogleSigninService> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _jwtService = jWTService;
            _logger = logger;
        }

        public async Task<TokenResult?> CreateToken(AuthenticateResult authenticateResult)
        {
            if (authenticateResult != null)
            {
                var claims = authenticateResult.Principal?.Claims;

                if (claims != null)
                {
                    var token = _jwtService.GenerateJwtToken(claims);
                    var email = authenticateResult.Principal?.FindFirstValue(System.Security.Claims.ClaimTypes.Email);

                    var result = new TokenResult { Token = token, Email = email };

                    return result;
                }
            }

            return null;
        }

        public async Task CreateUser(AuthenticateResult authenticateResult)
        {
            if (authenticateResult != null)
            {
                var claims = authenticateResult.Principal?.Claims;

                var email = authenticateResult.Principal?.FindFirstValue(System.Security.Claims.ClaimTypes.Email);
                var firstName = authenticateResult.Principal?.FindFirstValue(System.Security.Claims.ClaimTypes.GivenName);
                var lastName = authenticateResult.Principal?.FindFirstValue(System.Security.Claims.ClaimTypes.Surname);

                if (claims != null)
                {
                    var token = _jwtService.GenerateJwtToken(claims);

                    var user = await _userManager.FindByEmailAsync(email);

                    if (user == null)
                    {
                        try
                        {
                            user = new ApplicationUser
                            {
                                Email = email,
                                UserName = email,
                                FirstName = firstName ?? null,
                                LastName = lastName ?? null,
                                AuthenticationMethod = "Google"
                            };

                            await _userManager.CreateAsync(user);
                        }
                        catch (Exception e)
                        {
                            _logger.LogError($"Error creating user: {email}: {e.Message}");
                            throw;
                        }
                    }
                }
            }
        }
    }
}