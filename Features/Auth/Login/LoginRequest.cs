namespace Vyracare.Auth.Features.Auth.Login;

/// <summary>
/// Define o contrato de entrada esperado por este caso de uso.
/// </summary>
public sealed record LoginRequest(string Email, string Password);
