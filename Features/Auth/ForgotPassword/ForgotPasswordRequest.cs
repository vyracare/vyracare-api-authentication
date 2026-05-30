namespace Vyracare.Auth.Features.Auth.ForgotPassword;

/// <summary>
/// Define o contrato de entrada esperado por este caso de uso.
/// </summary>
public sealed record ForgotPasswordRequest(string Email, string Password);
