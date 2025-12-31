using System.ComponentModel.DataAnnotations;

namespace Ratbags.Accounts.Models.API.Register;

public class ResendEmailConfirmation
{
    [Required(ErrorMessage = "Email address is required")]
    public string Email { get; set; } = default!;
}
