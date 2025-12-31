using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Ratbags.Accounts.API.Models;
using Ratbags.Accounts.Interfaces;
using Swashbuckle.AspNetCore.Annotations;
using System.Net;

namespace Ratbags.Accounts.Controllers;

[ApiController]
[Route("api/accounts/login")]
public class LoginController : ControllerBase
{
    private readonly ILoginService _loginService;
    private readonly AppSettings _appSettings;
    private readonly ILogger<LoginController> _logger;

    public LoginController(
        ILoginService loginService,
        AppSettings appSettings,
        ILogger<LoginController> logger)
    {
        _loginService = loginService;
        _appSettings = appSettings;
        _logger = logger;
    }

    [HttpPost]
    [ProducesResponseType(typeof(string), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [SwaggerOperation(Summary = "Log in with email and password",
        Description = "Returns a jwt token result (jwt, email userid) and sets a refresh token in a cookie")]
    public async Task<IActionResult> Post([FromBody] LoginRequest model)
    {
        if (model == null)
        {
            _logger.LogWarning($"User details missing from login request");
            return BadRequest("Invalid login details");
        }

        var result = await _loginService.Login(model);

        if (result == null)
        {
            return Unauthorized();
        }

        // return refresh token in a http cookie to stop js reading it
        HttpContext.Response.Cookies.Append("refreshToken", result.RefreshToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = false,   // TODO set to true in live!
            SameSite = SameSiteMode.Strict, // prevents cookie being sent in cross-site requests
            Expires = DateTime.UtcNow.AddMinutes(_appSettings.TokenExpiry.RefreshTokenExpiryAddMinutes)
        });

        return Ok(new { result.JWT });
    }
}