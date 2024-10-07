using Ratbags.Core.DTOs.Account;

namespace Ratbags.Account.Interfaces;

public interface ILoginService
{
    Task<string?> Login(LoginDTO model);
}