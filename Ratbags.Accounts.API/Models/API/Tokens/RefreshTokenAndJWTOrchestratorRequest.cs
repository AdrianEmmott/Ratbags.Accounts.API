using Ratbags.Accounts.API.Models.DB;
using System.ComponentModel.DataAnnotations;

namespace Ratbags.Accounts.API.Models.API.Tokens;

public class RefreshTokenAndJWTOrchestratorRequest
{
    [Required(ErrorMessage = "User is required")]
    public ApplicationUser User { get; set; } = default!;

    [Required(ErrorMessage = "Existing refresh token is required")]
    public string? ExistingRefreshToken { get; set; }
}
