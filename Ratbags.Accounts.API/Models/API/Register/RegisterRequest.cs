using System.ComponentModel.DataAnnotations;

namespace Ratbags.Accounts.Models.API.Register;

public class RegisterRequest
{
    [Required(ErrorMessage = "First name is required")]
    public string FirstName { get; set; } = default!;

    [Required(ErrorMessage = "Last name is required")]
    public string LastName { get; set; } = default!;

    [Required(ErrorMessage = "Email is required")]
    public string Email { get; set; } = default!;

    [Required(ErrorMessage = "Password is required")]
    public string Password { get; set; } = default!;
}
