using System.ComponentModel.DataAnnotations;

namespace Ratbags.Accounts.API.Models.API
{
    public class RefreshTokenCreateRequest
    {
        [Required(ErrorMessage = "User id is required")]
        public Guid UserId { get; set; }
    }
}
