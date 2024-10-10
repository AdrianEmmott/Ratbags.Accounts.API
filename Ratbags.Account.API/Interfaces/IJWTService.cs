using Ratbags.Account.Models;
using System.Security.Claims;

namespace Ratbags.Account.Interfaces;

public interface IJWTService
{
    string GenerateJwtToken(ApplicationUser user);

    string GenerateJwtToken(IEnumerable<Claim> claims);
}