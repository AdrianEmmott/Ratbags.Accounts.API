using Microsoft.AspNetCore.Identity;

namespace Ratbags.Accounts.API.Models.DB
{
    public class ApplicationUser : IdentityUser
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }

        public string? AuthenticationMethod { get; set; }
    }
}
