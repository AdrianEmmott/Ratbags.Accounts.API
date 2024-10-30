using System.ComponentModel.DataAnnotations;

namespace Ratbags.Accounts.API.Models.API.Tokens;

public class RefreshTokenRequest
{
    [Required(ErrorMessage = "User id is required")]
    public Guid UserId { get; set; }
}

