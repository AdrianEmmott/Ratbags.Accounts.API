using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Ratbags.Account.Models;
using Ratbags.Core.DTOs.Account;
using Swashbuckle.AspNetCore.Annotations;
using System.Net;

namespace Ratbags.Account.Controllers;

[ApiController]
[Route("api/account/register")]
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
    [SwaggerOperation(Summary = "Registers a user",
        Description = "Registers a user")]
    public async Task<IActionResult> Post([FromBody] RegisterDTO model)
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
            return Created();
        }

        _logger.LogError($"error occured registering user {model.Email}: {result.Errors}");
        return BadRequest(result.Errors);
    }
}