using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Facebook;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Mvc;
using Ratbags.Account.API.Interfaces;
using Ratbags.Account.API.Models;
using Swashbuckle.AspNetCore.Annotations;
using System.Net;

namespace Ratbags.Account.Controllers;

/// <summary>
/// Authenticate user with Facebook.
/// </summary>
/// <remarks>
/// Most of this gets handled by AuthenticationServiceExtension, which is called by program.cs
/// </remarks>
[ApiController]
[Route("api/accounts/external-authentication")]
public class ExternalAuthenticationController : ControllerBase
{
    private readonly IExternalSigninService _signinService;
    private readonly ILogger<LoginController> _logger;
    

    public ExternalAuthenticationController(
        IExternalSigninService signinService,
        ILogger<LoginController> logger)
    {
        _signinService = signinService;
        _logger = logger;
    }

    
    /// <summary>
    /// Kicks off external authentication process with a challenge
    /// </summary>
    /// <param name="providerName">External provider name (e.g. Google / Facebook) is used to store login method when creating user 
    /// and used on callback to update ui
    /// </param>
    /// <returns></returns>
    [HttpGet("sign-in/{providerName}")]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [SwaggerOperation(Summary = "Kicks off external authentication process with a challenge",
        Description = "Returns a bad request if provider name is invalid")]
    public IActionResult Signin(string providerName)
    {
        // TODO make providerName an enum
        // TODO add client identifier - angular app etc - for client callback url (in callback) - no rush...

        var redirectUrl = Url.Action("Callback", "ExternalAuthentication", new { providerName = providerName });

        var properties = new AuthenticationProperties { RedirectUri = redirectUrl };

        return providerName.ToLowerInvariant() switch
        {
            "google" => Challenge(properties, GoogleDefaults.AuthenticationScheme),
            "facebook" => Challenge(properties, FacebookDefaults.AuthenticationScheme),
            _ => BadRequest()
        };
    }

    [HttpGet("callback")]
    [ProducesResponseType((int)HttpStatusCode.Redirect)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [SwaggerOperation(Summary = "Callback url for external authentication",
        Description = "If authentication succeeds, redirect to client, which should then call the Token method")]
    public async Task<IActionResult> Callback(string providerName)
    {
        var authenticateResult = await HttpContext.AuthenticateAsync
            (CookieAuthenticationDefaults.AuthenticationScheme);

        if (!authenticateResult.Succeeded)
        {
            return BadRequest();
        }

        // redirect to angular - 
        // TODO have a callback url to handle multiple clients
        var redirectUrl = $"https://localhost:4200/external-sign-in-callback/{providerName}";
        return Redirect(redirectUrl);
    }

    [HttpGet("token/{providerName}")]
    [ProducesResponseType(typeof(TokenResult), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [SwaggerOperation(Summary = "Creates a JWT token from the external authentication callback result",
        Description = "Returns token and email and creates a user in the system if they don't exist")]
    public async Task<IActionResult> Token(string providerName)
    {
        var authenticateResult = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

        if (!authenticateResult.Succeeded)
        {
            return BadRequest();
        }

        var token = await _signinService.CreateToken(authenticateResult);

        await _signinService.CreateUser(authenticateResult, providerName);

        return Ok(token);
    }
}