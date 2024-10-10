using Microsoft.AspNetCore.Identity;

namespace Ratbags.Account.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }

        public string? AuthenticationMethod { get; set; }
    }
}
