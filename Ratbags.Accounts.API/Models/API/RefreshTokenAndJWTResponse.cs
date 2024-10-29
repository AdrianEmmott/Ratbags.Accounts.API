using System.ComponentModel.DataAnnotations;

namespace Ratbags.Accounts.API.Models.API;

public class RefreshTokenAndJWTResponse
{
    [Required(ErrorMessage = "Refresh token is required")]
    public string RefreshToken { get; set; }

    [Required(ErrorMessage = "JWT result is required")]
    public string JWT { get; set; }

}
