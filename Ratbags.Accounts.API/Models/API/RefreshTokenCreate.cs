using System.ComponentModel.DataAnnotations;

namespace Ratbags.Accounts.API.Models.API
{
    public class RefreshTokenCreate
    {
        public Guid Id { get; set; }

        [Required(ErrorMessage = "User id is required")]
        public Guid UserId { get; set; }

        public DateTime Created { get; set; }

        public DateTime Expires { get; set; }

        public string Token { get; set; } = string.Empty;
    }
}
