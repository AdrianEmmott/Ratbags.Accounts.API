using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Ratbags.Accounts.API.Models;
using Ratbags.Accounts.API.Interfaces;
using Ratbags.Accounts.API.Models.DB;
using Ratbags.Core.Models.Accounts;
using Swashbuckle.AspNetCore.Annotations;
using System.Net;
using System.Web;

namespace Ratbags.Accounts.Controllers;

[ApiController]
[Route("api/accounts/register")]
public class RegisterController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IMassTransitService _massTransitService;
    private readonly ILogger<RegisterController> _logger;

    public RegisterController(
        UserManager<ApplicationUser> userManager,
        ILogger<RegisterController> logger,
        IMassTransitService massTransitService)
    {
        _userManager = userManager;
        _massTransitService = massTransitService;
        _logger = logger;
    }

    [HttpPost]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(IEnumerable<IdentityError>), (int)HttpStatusCode.BadRequest)]
    [SwaggerOperation(Summary = "Registers a user and sends confirmation email",
        Description = "Registers a user and sends confirmation email. Users cannot login until email has been confirmed")]
    public async Task<IActionResult> Post([FromBody] RegisterModel model)
    {
        var user = new ApplicationUser
        {
            UserName = model.Email,
            Email = model.Email,
            FirstName = model.FirstName,
            LastName = model.LastName,
        };

        var result = await _userManager.CreateAsync(user, model.Password);

        if (result.Succeeded)
        {
            _logger.LogInformation($"registered user {model.Email}: {user.Id}");

            // create confirm email token
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var encodedToken = HttpUtility.UrlEncode(token);

            await _massTransitService.SendRegisterConfirmEmailRequest(user.FirstName ?? null, user.Email, Guid.Parse(user.Id), encodedToken);

            return Ok();
        }

        _logger.LogError($"error occured registering user {model.Email}: {result.Errors}");
        return BadRequest("Error registering user");
    }


    [HttpPost("confirm-email")]
    [ProducesResponseType((int)HttpStatusCode.Created)]
    [ProducesResponseType(typeof(IEnumerable<IdentityError>), (int)HttpStatusCode.BadRequest)]
    [SwaggerOperation(Summary = "Confirms a user's email address", Description = "Expects user id / token")]
    public async Task<IActionResult> ConfirmEmail([FromBody] RegisterConfirmEmailModel model)
    {
        var user = await _userManager.FindByIdAsync(model.UserId.ToString());

        if (user == null)
        {
            return BadRequest("Invalid user id");
        }

        var result = await _userManager.ConfirmEmailAsync(user, model.Token);

        if (result.Succeeded)
        {
            _logger.LogInformation($"email confirmed for user {user.Email}");
            return Ok(true);
        }
        
        _logger.LogError($"error confirming email for user {user.Email}: {result.Errors}");
        return BadRequest("Error confirming email.");
    }


    [HttpPost("resend-confirm-email")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(IEnumerable<IdentityError>), (int)HttpStatusCode.BadRequest)]
    [SwaggerOperation(Summary = "Resends confirmation email",
        Description = "Resends confirmation email in case original email lost")]
    public async Task<IActionResult> ResendConfirmationEmail([FromBody] ResendEmailConfirmationModel model)
    {
        var user = await _userManager.FindByEmailAsync(model.Email);

        if (user != null)
        {
            _logger.LogInformation($"resending confirmation email for user {user.Email}: {user.Id}");

            // create confirm email token
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var encodedToken = HttpUtility.UrlEncode(token);
            var confirmUrl = $"https://localhost:4200/register-confirm-email/{user.Id}/{encodedToken}";

            _logger.LogInformation($"regiter confirm url: {confirmUrl}");

            return Ok(new { confirmUrl });
        }

        _logger.LogWarning($"resending email confirmation - could not find user {model.Email}");
        return BadRequest("Error resending confirmation email");
    }
}