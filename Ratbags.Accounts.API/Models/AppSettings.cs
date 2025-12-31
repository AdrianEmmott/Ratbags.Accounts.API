using Ratbags.Core.Settings;

namespace Ratbags.Accounts.API.Models;

public class AppSettings : AppSettingsBase
{
    public TokenExpiry TokenExpiry { get; set; } = default!;   

    public MessagingExtentensions MessagingExtensions { get; set; } = default!;
}

public class MessagingExtentensions
{
    public string ForgotPasswordEmailRequestTopic { get; set; } = default!;

    public string UserNameDetailsRequestTopic { get; set; } = default!;
}

public class TokenExpiry
{
    public int RefreshTokenExpiryAddMinutes { get; set; }
    public int JWTExpiryAddMinutes { get; set; }
}