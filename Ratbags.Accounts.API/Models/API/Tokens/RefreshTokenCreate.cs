using System.ComponentModel.DataAnnotations;

namespace Ratbags.Accounts.API.Models.API.Tokens;

public class RefreshTokenCreate
{
    [Required(ErrorMessage = "Id is required")]
    public Guid Id { get; set; }

    [Required(ErrorMessage = "User id is required")]
    public Guid UserId { get; set; }

    [Required(ErrorMessage = "Created (dateTime) is required")]
    public DateTime Created { get; set; }

    [Required(ErrorMessage = "Expires (dateTime) is required")]
    public DateTime Expires { get; set; }

    [Required(ErrorMessage = "Token is required")]
    public string Token { get; set; } = string.Empty;
}
