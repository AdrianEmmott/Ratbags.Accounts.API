using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Ratbags.Account.API.Interfaces;
using Ratbags.Account.API.Models;
using Ratbags.Account.Interfaces;
using Ratbags.Account.Models;
using Swashbuckle.AspNetCore.Annotations;
using System.Net;

namespace Ratbags.Account.Controllers;

/// <summary>
/// Authenticate user with Google.
/// </summary>
/// <remarks>
/// Most of this gets handled by AuthenticationServiceExtension, which is called by program.cs.
/// 
/// NB: This won't work on a local docker container as console.cloud.google doesn't like non public callback URIs
/// </remarks>
[ApiController]
[Route("api/accounts/google")]
public class GoogleSigninController : ControllerBase
{
    private readonly ILoginService _loginService;
    private readonly IGoogleSigninService _googleLoginService;
    private readonly ILogger<LoginController> _logger;

    public GoogleSigninController(
        ILoginService loginService,
        IGoogleSigninService googleLoginService,
        ILogger<LoginController> logger)
    {
        _loginService = loginService;
        _googleLoginService = googleLoginService;
        _logger = logger;
    }

    // TODO add client identifier - angular app etc - for client callback url (in callback)...
    [HttpGet("sign-in")]
    [SwaggerOperation(Summary = "Kicks off Google authentication process with a challenge",
        Description = "")]
    public IActionResult Signin()
    {
        var redirectUrl = Url.Action("Callback", "GoogleSignin");
        var properties = new AuthenticationProperties { RedirectUri = redirectUrl };
        return Challenge(properties, GoogleDefaults.AuthenticationScheme);
    }

    [HttpGet("callback")]
    [ProducesResponseType((int)HttpStatusCode.Redirect)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [SwaggerOperation(Summary = "Callback url for Google authentication",
        Description = "If authentication succeeds, redirect to client, which should then call the Token method")]
    public async Task<IActionResult> Callback()
    {
        var authenticateResult = await HttpContext.AuthenticateAsync
            (CookieAuthenticationDefaults.AuthenticationScheme);

        if (!authenticateResult.Succeeded)
        {
            return BadRequest();
        }

        // redirect to angular - there must be a better way!
        // think i'll have a callback url when sign-in called
        var redirectUrl = "https://localhost:4200/google-signin-callback";
        return Redirect(redirectUrl);
    }

    [HttpGet("token")]
    [ProducesResponseType(typeof(TokenResult), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [SwaggerOperation(Summary = "Creates a JWT token from the Google authentication callback result",
        Description = "Returns token and email and creates a user in the system if they don't exist")]
    public async Task<IActionResult> Token()
    {
        var authenticateResult = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

        if (!authenticateResult.Succeeded)
        {
            return BadRequest();
        }

        var token = await _googleLoginService.CreateToken(authenticateResult);

        await _googleLoginService.CreateUser(authenticateResult);

        return Ok(token);
    }
}