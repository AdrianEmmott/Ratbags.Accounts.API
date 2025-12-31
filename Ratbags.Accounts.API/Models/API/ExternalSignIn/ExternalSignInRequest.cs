using Microsoft.AspNetCore.Authentication;
using System.ComponentModel.DataAnnotations;

namespace Ratbags.Accounts.API.Models.Accounts.ExternalSignIn;

public class ExternalSignInRequest
{
    [Required(ErrorMessage = "AuthenticateResult is required")]
    public AuthenticateResult AuthenticateResult { get; set; } = default!;

    [Required(ErrorMessage = "ProviderName is required")]
    public string ProviderName { get; set; } = default!;
}
