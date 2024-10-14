using Microsoft.AspNetCore.Mvc;
using Ratbags.Account.Interfaces;
using Ratbags.Core.Models.Accounts;
using Swashbuckle.AspNetCore.Annotations;
using System.Net;

namespace Ratbags.Account.Controllers;

[ApiController]
[Route("api/accounts/login")]
public class LoginController : ControllerBase
{
    private readonly ILoginService _loginService;
    private readonly ILogger<LoginController> _logger;

    public LoginController(
        ILoginService loginService,
        ILogger<LoginController> logger)
    {
        _loginService = loginService;
        _logger = logger;
    }

    [HttpPost]
    [ProducesResponseType(typeof(string), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [SwaggerOperation(Summary = "Log in with email and password",
        Description = "Returns a token result - token and email")]
    public async Task<IActionResult> Post([FromBody] LoginModel model)
    {
        var token = await _loginService.Login(model);

        if (token != null)
        {
            return Ok(token);
        }

        return Unauthorized();
    }
}