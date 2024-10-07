using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Ratbags.Account.Interfaces;
using Ratbags.Account.Models;
using Ratbags.Shared.DTOs.Events.DTOs.Account;
using Swashbuckle.AspNetCore.Annotations;
using System.Net;
using System.Runtime.CompilerServices;
using System.Security.Claims;

namespace Ratbags.Account.Controllers;

[ApiController]
[Route("api/account")]
public class AccountController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IJWTService _jwtService;
    private readonly ILoginService _loginService;
    private readonly ILogger<AccountController> _logger;

    public AccountController(UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        IJWTService jWTService,
        ILoginService loginService,
        ILogger<AccountController> logger)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _jwtService = jWTService;
        _loginService = loginService;
        _logger = logger;
    }

    [HttpPost("register")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(IEnumerable<IdentityError>), (int)HttpStatusCode.BadRequest)]
    [SwaggerOperation(Summary = "Gets all articles",
        Description = "Returns a list of all articles or an empty list")]
    public async Task<IActionResult> Register([FromBody] RegisterDTO model)
    {
        var user = new ApplicationUser { UserName = model.Email, Email = model.Email };

        var result = await _userManager.CreateAsync(user, model.Password);

        if (result.Succeeded)
        {
            return Ok();
        }

        return BadRequest(result.Errors);
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
        _logger.LogInformation("validationg token");
        
        // if we get here, token is valid - authorize tag will block otherwise
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;

        return Ok(new { UserId = userId, Email = userEmail });
    }
}