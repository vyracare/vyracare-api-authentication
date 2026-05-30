namespace Vyracare.Auth.Features.Auth.FirstAccessCheck;

public sealed record FirstAccessCheckResponse(bool Exists, bool CanSetPassword);
