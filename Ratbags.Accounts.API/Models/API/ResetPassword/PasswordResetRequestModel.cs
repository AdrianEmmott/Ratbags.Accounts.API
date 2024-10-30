using System.ComponentModel.DataAnnotations;

namespace Ratbags.Accounts.API.Models.ResetPassword;

public class PasswordResetRequestModel
{
    [Required(ErrorMessage = "Email is required")]
    public string Email { get; set; } = null!;
}
