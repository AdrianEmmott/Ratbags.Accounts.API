using System.ComponentModel.DataAnnotations;

namespace Ratbags.Accounts.API.Models.ResetPassword;

public class PasswordResetUpdateModel
{
    [Required(ErrorMessage = "User id is required")]
    public Guid UserId { get; set; }

    [Required(ErrorMessage = "PasswordResetToken id is required")]
    public string PasswordResetToken { get; set; } = default!;

    [Required(ErrorMessage = "Password is required")]
    public string Password { get; set; } = default!;
}
