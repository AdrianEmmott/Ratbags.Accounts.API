using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Ratbags.Accounts.API.Models.DB;
using Ratbags.Core.Models.Accounts;
using Swashbuckle.AspNetCore.Annotations;
using System.Net;
using System.Text.Encodings.Web;
using System.Web;

namespace Ratbags.Account.Controllers;

[ApiController]
[Route("api/accounts/reset-password")]
public class ResetPasswordController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<ResetPasswordController> _logger;

    public ResetPasswordController(UserManager<ApplicationUser> userManager,
        ILogger<ResetPasswordController> logger)
    {
        _userManager = userManager;
        _logger = logger;
    }

    [HttpPost("request")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [SwaggerOperation(Summary = "Handles password reset requests",
        Description = @"Handles password reset request and creates password reset email if user exists. 
                        Return Ok if user is found / not found, so we don't acknowledge user's existence. 
                        However, it will return bad request if email is missing")]
    public async Task<IActionResult> ResetRequest([FromBody] PasswordResetRequestModel model)
    {
        var user = await _userManager.FindByEmailAsync(model.Email);

        var resetUrl = string.Empty;

        if (user != null)
        {
            // create password reset token
            var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
            var encodedToken = HttpUtility.UrlEncode(resetToken);

            resetUrl = $"https://localhost:4200/reset-password/{user.Id}/{encodedToken}";

            // TODO email user - use papercut?

            _logger.LogInformation($"user {model.Email} requested a password reset");
        }
        else
        {
            _logger.LogInformation($"user {model.Email} requested a password reset - user does not exist");
        }

        return Ok(new {resetUrl}); // debug for now
        //return Ok(); // real line once emails are working
    }

    [HttpPost("update")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [SwaggerOperation(Summary = "Handles password reset updates",
        Description = "Handles password reset updates")]
    public async Task<IActionResult> Update([FromBody] PasswordResetUpdateModel model)
    {
        var user = await _userManager.FindByIdAsync(model.UserId.ToString());
        
        if (user == null)
        {
            return BadRequest();
        }

        var result = await _userManager.ResetPasswordAsync(user, model.PasswordResetToken.ToString(), model.Password);

        if (result.Succeeded)
        {
            return Ok(result.Succeeded);
        }

        return BadRequest();
    }
}