using System.ComponentModel.DataAnnotations;

namespace Ratbags.Accounts.Models.API.Register;

public class RegisterConfirmEmail
{
    [Required(ErrorMessage = "User id is required")]
    public Guid UserId { get; set; }

    [Required(ErrorMessage = "Token is required")]
    public string Token { get; set; } = default!;
}
