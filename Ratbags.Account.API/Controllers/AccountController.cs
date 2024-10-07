using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Ratbags.Account.Interfaces;
using Ratbags.Account.Models;
using Ratbags.Core.DTOs.Account;
using Swashbuckle.AspNetCore.Annotations;
using System.Net;
using System.Security.Claims;

namespace Ratbags.Account.Controllers;

[ApiController]
[Route("api/account")]
public class AccountController : ControllerBase
{
    private readonly ILoginService _loginService;
    private readonly ILogger<AccountController> _logger;

    public AccountController(UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        IJWTService jWTService,
        ILoginService loginService,
        ILogger<AccountController> logger)
    {
        _loginService = loginService;
        _logger = logger;
    }

    [HttpPost("login")]
    [ProducesResponseType(typeof(string), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [SwaggerOperation(Summary = "Log in with email and password",
        Description = "Returns a JWT token")]
    public async Task<IActionResult> Login([FromBody] LoginDTO model)
    {
        var token = await _loginService.Login(model);

        if (!string.IsNullOrEmpty(token))
        {
            return Ok(new { token = token });
        }

        return Unauthorized();
    }

    [Authorize]
    [HttpGet("validate-token")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    public IActionResult ValidateToken()
    {
        _logger.LogInformation("validating token");
        
        // if we get here, token is valid - authorize tag will block otherwise
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;

        return Ok(new { UserId = userId, Email = userEmail });
    }
}