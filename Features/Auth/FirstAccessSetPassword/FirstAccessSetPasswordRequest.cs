namespace Vyracare.Auth.Features.Auth.FirstAccessSetPassword;

/// <summary>
/// Define o contrato de entrada ou saída usado por esta feature.
/// </summary>
public sealed record FirstAccessSetPasswordRequest(string Email, string Password);
