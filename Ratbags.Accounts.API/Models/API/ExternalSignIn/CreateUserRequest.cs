using Microsoft.AspNetCore.Authentication;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace Ratbags.Accounts.API.Models.Accounts.ExternalSignIn;

public class CreateUserRequest
{
    [Required(ErrorMessage = "Claims is required")]
    public IEnumerable<Claim> Claims { get; set; } = default!;

    [Required(ErrorMessage = "ProviderName is required")]
    public string ProviderName { get; set; } = default!;

    [Required(ErrorMessage = "Email is required")]
    public string Email { get; set; } = default!;
}
