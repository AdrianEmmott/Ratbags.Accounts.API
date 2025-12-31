using Ratbags.Accounts.API.Models.ResetPassword;

namespace Ratbags.Accounts.Interfaces;

public interface IResetPasswordService
{
    Task<bool> ResetRequest(PasswordResetRequest model);

    Task<bool> Update(PasswordResetUpdate model);
}