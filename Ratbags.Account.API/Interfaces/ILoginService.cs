using Ratbags.Shared.DTOs.Events.DTOs.Account;

namespace Ratbags.Account.Interfaces;

public interface ILoginService
{
    Task<string?> Login(LoginDTO model);
}