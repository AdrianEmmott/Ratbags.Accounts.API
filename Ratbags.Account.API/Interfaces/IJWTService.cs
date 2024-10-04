using Ratbags.Account.Models;

namespace Ratbags.Account.Interfaces;

public interface IJWTService
{
    string GenerateJwtToken(ApplicationUser user);
}