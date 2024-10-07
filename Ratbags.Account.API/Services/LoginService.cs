﻿using Microsoft.AspNetCore.Identity;
using Ratbags.Account.Interfaces;
using Ratbags.Account.Models;
using Ratbags.Shared.DTOs.Events.DTOs.Account;

namespace Ratbags.Account.Services
{
    public class LoginService : ILoginService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IJWTService _jwtService;

        public LoginService(UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IJWTService jWTService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _jwtService = jWTService;
        }

        public async Task<string?> Login(LoginDTO model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user != null)
            {
                var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);

                if (result.Succeeded)
                {
                    var token = _jwtService.GenerateJwtToken(user);
                    return token;
                }
            }

            return null;
        }
    }
}