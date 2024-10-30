using Ratbags.Accounts.API.Models.ResetPassword;

namespace Ratbags.Accounts.Interfaces;

public interface IResetPasswordService
{
    Task<bool> ResetRequest(PasswordResetRequestModel model);

    Task<bool> Update(PasswordResetUpdateModel model);
}