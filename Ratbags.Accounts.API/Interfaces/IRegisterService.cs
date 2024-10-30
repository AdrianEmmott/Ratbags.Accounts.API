using Ratbags.Accounts.API.Models;
using Ratbags.Accounts.Models.API.Register;

namespace Ratbags.Accounts.Interfaces;

public interface IRegisterService
{
    Task<bool> Register(RegisterModel model);

    Task<bool> RegisterComfirm(RegisterConfirmEmailModel model);

    Task<bool> ResendConfirmationEmail(ResendEmailConfirmationModel model);
}