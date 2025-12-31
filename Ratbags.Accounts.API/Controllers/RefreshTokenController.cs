using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Ratbags.Accounts.API.Interfaces;
using Ratbags.Accounts.API.Models;
using Ratbags.Accounts.API.Models.API.Tokens;
using Ratbags.Accounts.API.Models.DB;
using Ratbags.Accounts.Interfaces;
using Swashbuckle.AspNetCore.Annotations;
using System.Net;

namespace Ratbags.Accounts.Controllers;

[ApiController]
[Route("api/accounts/refresh-token")]
public class RefreshTokenController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IRefreshTokenService _refreshTokenService;
    private readonly IRefreshAndJWTOrchestrator _refreshAndJWTResponseOrchestrator;
    private readonly AppSettings _appSettings;
    private readonly ILogger<RefreshTokenController> _logger;
    
    public RefreshTokenController(
        UserManager<ApplicationUser> userManager,
        IRefreshAndJWTOrchestrator refreshAndJWTResponseOrchestrator,
        IRefreshTokenService refreshTokenService,
        AppSettings appSettings,
        ILogger<RefreshTokenController> logger)
    {
        _userManager = userManager;
        _refreshTokenService = refreshTokenService;
        _refreshAndJWTResponseOrchestrator = refreshAndJWTResponseOrchestrator;
        _appSettings = appSettings;
        _logger = logger;
    }

    [HttpPost]
    [ProducesResponseType(typeof(string), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [SwaggerOperation(Summary = "Attempts to create a refresh token for a user",
        Description = "Returns a jwt and creates a refresh token cookie")]
    public async Task<IActionResult> Post([FromBody] RefreshTokenRequest model)
    {
        _logger.LogInformation($"refresh token request");

        // get current refresh token cookie
        var cookie = HttpContext.Request.Cookies["refreshToken"];

        if (cookie == null)
        {
            return BadRequest("Invalid existing refresh token cookie");
        }

        var user = await _userManager.FindByIdAsync(model.UserId.ToString());

        if (user == null)
        {
            return BadRequest("Invalid user id");
        }

        var result = await _refreshAndJWTResponseOrchestrator
            .CreateResponseAsync(new RefreshTokenAndJWTOrchestratorRequest
            {
                User = user,
                ExistingRefreshToken = cookie
            });

        if (result == null)
        {
            return Unauthorized();
        }

        _logger.LogInformation($"refresh token request success: {DateTime.UtcNow.ToString()}");

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