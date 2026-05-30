namespace Vyracare.Auth.Features.Auth.FirstAccessCheck;

/// <summary>
/// Define o contrato de sa?da retornado por este caso de uso.
/// </summary>
public sealed record FirstAccessCheckResponse(bool Exists, bool CanSetPassword);
