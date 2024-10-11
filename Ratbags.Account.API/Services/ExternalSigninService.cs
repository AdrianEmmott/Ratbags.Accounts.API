using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Ratbags.Account.API.Interfaces;
using Ratbags.Account.API.Models;
using Ratbags.Account.Interfaces;
using Ratbags.Account.Models;
using System.Security.Claims;

namespace Ratbags.Account.Services;

/// <summary>
/// Signing in via external provider - Google, Facebook etc
/// </summary>
public class ExternalSigninService : IExternalSigninService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IJWTService _jwtService;
    private readonly ILogger<ExternalSigninService> _logger;

    public ExternalSigninService(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        IJWTService jWTService,
        ILogger<ExternalSigninService> logger)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _jwtService = jWTService;
        _logger = logger;
    }

    /// <summary>
    /// Creates JWT Token based on claims handed over by external provider
    /// </summary>
    /// <param name="authenticateResult">Authentication result returned by external provider</param>
    /// <returns></returns>
    public async Task<TokenResult?> CreateToken(AuthenticateResult authenticateResult)
    {
        if (authenticateResult != null)
        {
            var claims = authenticateResult.Principal?.Claims;

            if (claims != null)
            {
                var token = _jwtService.GenerateJwtToken(claims);
                var email = authenticateResult.Principal?.FindFirstValue(System.Security.Claims.ClaimTypes.Email);

                // TODO should i be signing in the user here?!

                // TODO check for null email! - also we should probably return id, rather than email...
                var result = new TokenResult { Token = token, Email = email };

                return result;
            }
        }

        return null;
    }

    /// <summary>
    /// Creates user in system if one does not exist
    /// </summary>
    /// <param name="authenticateResult">Authentication result returned by external provider</param>
    /// <param name="providerName">Google, Facebook etc - so we know how the user signed in</param>
    /// <returns></returns>
    public async Task CreateUser(AuthenticateResult authenticateResult, string providerName)
    {
        if (authenticateResult != null)
        {
            // grab claims from authentication result
            var claims = authenticateResult.Principal?.Claims;

            // TODO should probably check if email is present earlier on - shouldn't create token without it
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
                            AuthenticationMethod = providerName,
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