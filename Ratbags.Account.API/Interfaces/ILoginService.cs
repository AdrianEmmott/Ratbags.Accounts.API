using Microsoft.AspNetCore.Authentication;
using Ratbags.Account.API.Models;
using Ratbags.Core.DTOs.Account;

namespace Ratbags.Account.Interfaces;

public interface ILoginService
{
    Task<TokenResult?> Login(LoginDTO model);
}