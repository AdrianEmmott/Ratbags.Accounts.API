using System.ComponentModel.DataAnnotations;

namespace Ratbags.Accounts.API.Models.API;

public class RefreshTokenResponse
{
    [Required(ErrorMessage = "User id is required")]
    public Guid UserId { get; set; }

    [Required(ErrorMessage = "New refresh token is required")]
    public string NewRefreshToken { get; set; }

    [Required(ErrorMessage = "JWT result is required")]
    public JWTAndUserDetailsResult JWTTokenResult { get; set; }

}
