using System.ComponentModel.DataAnnotations;

namespace Ratbags.Accounts.API.Models.ResetPassword;

public class PasswordResetRequest
{
    [Required(ErrorMessage = "Email is required")]
    public string Email { get; set; } = default!;
}
