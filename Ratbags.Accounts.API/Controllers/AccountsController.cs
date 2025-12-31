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
    [SwaggerOperation(Summary = "Validates a JWT against the server",
        Description = "Validates a JWT token against the server. If the JWT is invalid, Unauthorized is returned automatically")]
    public IActionResult ValidateToken()
    {
        _logger.LogInformation("validating token");

        // if we get here, token is valid - authorize tag will block otherwise
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;

        _logger.LogInformation($"validating token for {userEmail} ({userId})");

        return Ok(true);
    }
}