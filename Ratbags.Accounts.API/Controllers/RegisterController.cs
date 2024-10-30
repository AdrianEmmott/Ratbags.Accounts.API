using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Ratbags.Accounts.API.Models;
using Ratbags.Accounts.Interfaces;
using Ratbags.Accounts.Models.API.Register;
using Swashbuckle.AspNetCore.Annotations;
using System.Net;

namespace Ratbags.Accounts.Controllers;

[ApiController]
[Route("api/accounts/register")]
public class RegisterController : ControllerBase
{
    private readonly IRegisterService _registerService;
    private readonly ILogger<RegisterController> _logger;

    public RegisterController(
        IRegisterService registerService,
        ILogger<RegisterController> logger)
    {
        _registerService = registerService;
        _logger = logger;
    }

    [HttpPost]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
    [SwaggerOperation(Summary = "Registers a user and sends confirmation email",
        Description = "Registers a user and sends confirmation email. Users cannot login until email has been confirmed")]
    public async Task<IActionResult> Post([FromBody] RegisterModel model)
    {
        var result = await _registerService.Register(model);

        return result ? Ok(result) : BadRequest("Error registering user");
    }

    [HttpPost("confirm-email")]
    [ProducesResponseType((int)HttpStatusCode.Created)]
    [ProducesResponseType(typeof(IEnumerable<IdentityError>), (int)HttpStatusCode.BadRequest)]
    [SwaggerOperation(Summary = "Confirms a user's email address", 
        Description = "Expects user id / token")]
    public async Task<IActionResult> ConfirmEmail([FromBody] RegisterConfirmEmailModel model)
    {
        var result = await _registerService.RegisterComfirm(model);

        return result ? Ok(result) : BadRequest("Error confirming email.");
    }

    [HttpPost("resend-confirm-email")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(IEnumerable<IdentityError>), (int)HttpStatusCode.BadRequest)]
    [SwaggerOperation(Summary = "Resends confirmation email",
        Description = "Resends confirmation email in case original email lost")]
    public async Task<IActionResult> ResendConfirmationEmail([FromBody] ResendEmailConfirmationModel model)
    {
        var result = await _registerService.ResendConfirmationEmail(model);

        return result ? Ok(result) : BadRequest("Error resending confirmation email");
    }
}