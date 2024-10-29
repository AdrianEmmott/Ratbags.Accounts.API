using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Ratbags.Accounts.API.Interfaces;
using Ratbags.Accounts.API.Models.API;
using Ratbags.Accounts.API.Models.DB;
using Ratbags.Accounts.Interfaces;
using System.Security.Claims;

namespace Ratbags.Accounts.Services;

/// <summary>
/// Sign in in via external provider - Google, Facebook etc
/// </summary>
public class ExternalSigninService : IExternalSigninService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IRefreshAndJWTResponseOrchestrator _refreshAndJWTResponseOrchestrator;
    private readonly ILogger<ExternalSigninService> _logger;

    public ExternalSigninService(
        UserManager<ApplicationUser> userManager,
        IRefreshAndJWTResponseOrchestrator refreshAndJWTResponseOrchestrator,
        ILogger<ExternalSigninService> logger)
    {
        _userManager = userManager;
        _refreshAndJWTResponseOrchestrator = refreshAndJWTResponseOrchestrator;
        _logger = logger;
    }

    /// <summary>
    /// Attempts to login in a user using an external signing provider (e.g. Google)
    /// </summary>
    /// <param name="authenticateResult">
    /// Authentication result returned by external provider
    /// </param>
    /// <returns>LoginResponse or null</returns>
    public async Task<RefreshTokenAndJWTResponse?> Signin(AuthenticateResult authenticateResult, string providerName)
    {
        if (authenticateResult != null)
        {
            var claims = authenticateResult.Principal?.Claims;

            if (claims != null)
            {
                var email = authenticateResult.Principal?
                    .FindFirstValue(System.Security.Claims.ClaimTypes.Email);

                if (email == null)
                {
                    _logger.LogWarning($"external signin provider {providerName} did not provide email address for user");
                    return null;
                }

                var user = await _userManager.FindByEmailAsync(email);

                if (user == null)
                {
                    // create user
                    user = await CreateUser(claims, providerName, email);
                }

                var result = await _refreshAndJWTResponseOrchestrator.CreateResponseAsync(user);

                return result;
            }
        }

        return null;
    }

    /// <summary>
    /// Creates user in system if one does not exist
    /// </summary>
    /// <param name="authenticateResult">
    /// Authentication result returned by external provider
    /// </param>
    /// <param name="providerName">
    /// Google, Facebook etc - so we know how the user signed in
    /// </param>
    /// <returns></returns>
    public async Task<ApplicationUser?> CreateUser(IEnumerable<Claim> claims, string providerName, string email)
    {
        if (claims == null)
        {
            _logger.LogWarning($"Claims null whilst attempting to create user {email}, using provider {providerName}");
            return null;
        }

        if (!claims.Any(x => x.Type == System.Security.Claims.ClaimTypes.GivenName)
            || !claims.Any(x => x.Type == System.Security.Claims.ClaimTypes.Surname)) 
        {
            return null; 
        }

        var firstName = claims.FirstOrDefault(x => x.Type == ClaimTypes.GivenName).Value;
        var lastName = claims.FirstOrDefault(x => x.Type == ClaimTypes.Surname).Value;

        if (claims != null && email != null)
        {
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

                    return user;
                }
                catch (Exception e)
                {
                    _logger.LogError($"Error creating user: {email}: {e.Message}");
                    throw;
                }
            }
        }

        return null;
    }
}