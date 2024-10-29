using Ratbags.Accounts.API.Models.API;
using Ratbags.Accounts.API.Models.DB;
using System.Security.Claims;

namespace Ratbags.Accounts.Interfaces;

public interface IJWTService
{
    string GenerateJwtToken(ApplicationUser user);
}