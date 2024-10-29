using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ratbags.Accounts.API.Models.DB
{
    [Table("RefreshTokens")]
    public class RefreshToken
    {
        [Key]
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public DateTime Created { get; set; }
        public DateTime Expires { get; set; }
        public string Token { get; set; } = string.Empty;
        public bool Revoked { get; set; }
    }
}
