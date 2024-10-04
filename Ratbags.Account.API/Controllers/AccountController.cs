using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Ratbags.Account.Interfaces;
using Ratbags.Account.Models;
using Ratbags.Shared.DTOs.Events.DTOs.Account;

namespace Ratbags.Login.Controllers;

[Route("api/login")]
[ApiController]
public class AccountController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IJWTService _jwtService;

    public AccountController(UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        IJWTService jWTService)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _jwtService = jWTService;
    }

    [HttpPost("register")]
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
    public async Task<IActionResult> Login([FromBody] LoginDTO model)
    {
        var user = await _userManager.FindByEmailAsync(model.Email);
        if (user != null)
        {
            var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);
            if (result.Succeeded)
            {
                // Generate JWT Token
                var token = _jwtService.GenerateJwtToken(user);
                return Ok(new { token });
            }
        }

        return Unauthorized();
    }
}