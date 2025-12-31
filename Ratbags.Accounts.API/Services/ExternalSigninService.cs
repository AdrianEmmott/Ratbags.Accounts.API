using Microsoft.AspNetCore.Identity;
using Ratbags.Accounts.API.Interfaces;
using Ratbags.Accounts.API.Models.Accounts.ExternalSignIn;
using Ratbags.Accounts.API.Models.API.Tokens;
using Ratbags.Accounts.API.Models.DB;
using Ratbags.Accounts.Interfaces;
using System.Security.Claims;

namespace Ratbags.Accounts.API.Services;

/// <summary>
/// Sign in in via external provider - Google, Facebook etc
/// </summary>
public class ExternalSigninService : IExternalSigninService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IRefreshAndJWTOrchestrator _refreshAndJWTResponseOrchestrator;
    private readonly ILogger<ExternalSigninService> _logger;

    public ExternalSigninService(
        UserManager<ApplicationUser> userManager,
        IRefreshAndJWTOrchestrator refreshAndJWTResponseOrchestrator,
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
    /// <returns>RefreshTokenResponse or null</returns>
    public async Task<RefreshTokenAndJWTOrchestratorResponse?> SignIn(ExternalSignInRequest model)
    {
        if (model.AuthenticateResult != null)
        {
            var claims = model.AuthenticateResult.Principal?.Claims;

            if (claims != null)
            {
                var email = model.AuthenticateResult.Principal?
                    .FindFirstValue(System.Security.Claims.ClaimTypes.Email);

                if (email == null)
                {
                    _logger.LogWarning($"external signin provider {model.ProviderName} did not provide email address for user");
                    return null;
                }

                var user = await _userManager.FindByEmailAsync(email);

                if (user?.Email != null)
                {
                    // stop VS whining about nullable users
                    var nonNullUser = user;

                    // create user
                    user = await CreateUser(new CreateUserRequest 
                    { 
                        Claims = claims, 
                        ProviderName = model.ProviderName, 
                        Email = nonNullUser.Email 
                    });
                }

                if (user != null)
                {
                    var result = await _refreshAndJWTResponseOrchestrator
                        .CreateResponseAsync(new RefreshTokenAndJWTOrchestratorRequest { User = user });

                    return result;
                }
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
    public async Task<ApplicationUser?> CreateUser(CreateUserRequest model)
    {
        if (model.Claims == null)
        {
            _logger.LogWarning($"Claims null whilst attempting to create user {model.Email}, using provider {model.ProviderName}");
            return null;
        }

        if (model.Email == null)
        {
            _logger.LogWarning($"Email is missing, using provider {model.ProviderName}");
            return null;
        }

        var firstName = model.Claims.FirstOrDefault(x => x.Type == ClaimTypes.GivenName)?.Value ?? string.Empty;
        var lastName = model.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Surname)?.Value ?? string.Empty;

        var user = await _userManager.FindByEmailAsync(model.Email);

        if (user == null)
        {
            try
            {
                user = new ApplicationUser
                {
                    Email = model.Email,
                    UserName = model.Email,
                    FirstName = firstName ?? null,
                    LastName = lastName ?? null,
                    AuthenticationMethod = model.ProviderName,
                };

                await _userManager.CreateAsync(user);

                return user;
            }
            catch (Exception e)
            {
                _logger.LogError($"Error creating user: {model.Email}: {e.Message}");
                throw;
            }
        }
        
        return user;
    }
}