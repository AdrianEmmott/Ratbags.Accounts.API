namespace Ratbags.Accounts.API.Models.API
{
    public class JWTAndUserDetailsResult
    {
        public string Token { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
    }
}
