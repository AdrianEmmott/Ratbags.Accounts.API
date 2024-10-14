using Ratbags.Account.API.Models;
using Ratbags.Core.Models.Accounts;

namespace Ratbags.Account.Interfaces;

public interface ILoginService
{
    Task<TokenResult?> Login(LoginModel model);
}