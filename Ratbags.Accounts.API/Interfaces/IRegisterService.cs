using Ratbags.Accounts.API.Models;
using Ratbags.Accounts.Models.API.Register;

namespace Ratbags.Accounts.Interfaces;

public interface IRegisterService
{
    Task<bool> Register(RegisterRequest model);

    Task<bool> RegisterComfirm(RegisterConfirmEmail model);

    Task<bool> ResendConfirmationEmail(ResendEmailConfirmation model);
}