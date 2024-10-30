using System.ComponentModel.DataAnnotations;

namespace Ratbags.Accounts.API.Models.API.Tokens;

public class RefreshTokenAndJWTOrchestratorResponse
{
    [Required(ErrorMessage = "Refresh token is required")]
    public string RefreshToken { get; set; } = default!;

    [Required(ErrorMessage = "JWT result is required")]
    public string JWT { get; set; } = default!;

}
