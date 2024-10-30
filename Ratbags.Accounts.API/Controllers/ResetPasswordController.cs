using Microsoft.AspNetCore.Mvc;
using Ratbags.Accounts.Interfaces;
using Ratbags.Accounts.API.Models.ResetPassword;
using Swashbuckle.AspNetCore.Annotations;
using System.Net;

namespace Ratbags.Accounts.Controllers;

[ApiController]
[Route("api/accounts/reset-password")]
public class ResetPasswordController : ControllerBase
{
    private readonly IResetPasswordService _resetPasswordService;
    private readonly ILogger<ResetPasswordController> _logger;

    public ResetPasswordController(
        IResetPasswordService resetPasswordService,
        ILogger<ResetPasswordController> logger)
    {
        _resetPasswordService = resetPasswordService;
        _logger = logger;
    }

    [HttpPost("request")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [SwaggerOperation(Summary = "Handles password reset requests",
        Description = @"Handles password reset request and creates password reset email if user exists.")]
    public async Task<IActionResult> ResetRequest([FromBody] PasswordResetRequestModel model)
    {
        var result = await _resetPasswordService.ResetRequest(model);

        return result ? Ok(result) : BadRequest();
    }

    [HttpPost("update")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [SwaggerOperation(Summary = "Handles password reset updates",
        Description = "Handles password reset updates")]
    public async Task<IActionResult> Update([FromBody] PasswordResetUpdateModel model)
    {
        var result = await _resetPasswordService.Update(model);

        return result ? Ok(result) : BadRequest();
    }
}