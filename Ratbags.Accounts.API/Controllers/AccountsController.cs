using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Net;
using System.Security.Claims;

namespace Ratbags.Accounts.Controllers;

[ApiController]
[Route("api/accounts/")]
public class AccountsController : ControllerBase
{
    private readonly ILogger<LoginController> _logger;

    public AccountsController(
        ILogger<LoginController> logger)
    {
        _logger = logger;
    }

    [Authorize]
    [HttpGet("validate-token")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [SwaggerOperation(Summary = "Validates a JWT token against the server",
        Description = "Returns user id and email if JWT token valid / unauthorised if not")]
    public IActionResult ValidateToken()
    {
        _logger.LogInformation("validating token");

        // if we get here, token is valid - authorize tag will block otherwise
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;

        return Ok(new { UserId = userId, Email = userEmail });
    }
}