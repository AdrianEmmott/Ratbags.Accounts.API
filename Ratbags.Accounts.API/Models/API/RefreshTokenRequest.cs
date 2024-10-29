using System.ComponentModel.DataAnnotations;
using System.Net;

namespace Ratbags.Accounts.API.Models.API
{
    public class RefreshTokenRequest
    {
        [Required(ErrorMessage = "User id is required")]
        public Guid UserId { get; set; }

        [Required(ErrorMessage = "Refresh token is required")]
        public string CurrentRefreshToken { get; set; }

    }
}
