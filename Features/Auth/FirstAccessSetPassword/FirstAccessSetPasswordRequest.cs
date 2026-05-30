namespace Vyracare.Auth.Features.Auth.FirstAccessSetPassword;

/// <summary>
/// Define o contrato de entrada esperado por este caso de uso.
/// </summary>
public sealed record FirstAccessSetPasswordRequest(string Email, string Password);
