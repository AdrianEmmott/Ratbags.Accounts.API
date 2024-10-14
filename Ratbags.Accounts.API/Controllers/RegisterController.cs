using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Ratbags.Accounts.API.Models.DB;
using Ratbags.Core.Models.Accounts;
using Swashbuckle.AspNetCore.Annotations;
using System.Net;
using System.Web;

namespace Ratbags.Account.Controllers;

[ApiController]
[Route("api/accounts/register")]
public class RegisterController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<RegisterController> _logger;

    public RegisterController(UserManager<ApplicationUser> userManager,
        ILogger<RegisterController> logger)
    {
        _userManager = userManager;
        _logger = logger;
    }

    [HttpPost]
    [ProducesResponseType((int)HttpStatusCode.Created)]
    [ProducesResponseType(typeof(IEnumerable<IdentityError>), (int)HttpStatusCode.BadRequest)]
    [SwaggerOperation(Summary = "Registers a user and sends confirmation email",
        Description = "Registers a user and sends confirmation email. Users cannot login until email has been confirmed")]
    public async Task<IActionResult> Post([FromBody] RegisterModel model)
    {
        var user = new ApplicationUser
        {
            UserName = model.Email,
            Email = model.Email
        };

        var result = await _userManager.CreateAsync(user, model.Password);

        if (result.Succeeded)
        {
            _logger.LogInformation($"registered user {model.Email}: {user.Id}");

            // create confirm email token
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var encodedToken = HttpUtility.UrlEncode(token);
            var confirmUrl = $"https://localhost:4200/register-confirm-email/{user.Id}/{encodedToken}";

            return Ok(new { confirmUrl });
        }

        _logger.LogError($"error occured registering user {model.Email}: {result.Errors}");
        return BadRequest(result.Errors);
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
            return Ok(new { success = "email confirmed successfully"});
        }
        
        _logger.LogError($"error confirming email for user {user.Email}: {result.Errors}");
        return BadRequest("Error confirming email.");
    }
}