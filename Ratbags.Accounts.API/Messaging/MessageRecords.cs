namespace Ratbags.Accounts.API.Messaging;

public sealed record ForgotPasswordEmailRequest(
    string name,
    string email,
    Guid userId,
    string token);
public sealed record ForgotPasswordEmailResponse(bool success);

public sealed record GetUserNameDetailsRequest(IReadOnlyList<Guid> userIds);
public sealed record GetUserNameDetailsResponse(Dictionary<Guid, string>? userNameDetails);