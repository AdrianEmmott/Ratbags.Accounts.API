using Ratbags.Core.Settings;

namespace Ratbags.Accounts.API.Models;

public class AppSettings : AppSettingsBase
{
    public Tokens Tokens { get; set; } = new Tokens();
}

public class Tokens
{
    public RefreshTokens RefreshToken { get; set; } = new RefreshTokens();

    public JWTs JWT { get; set; } = new JWTs();
}

public class RefreshTokens
{
    public int ExpiryAddMinutes { get; set; }
}

public class JWTs
{
    public int ExpiryAddMinutes { get; set; }
}