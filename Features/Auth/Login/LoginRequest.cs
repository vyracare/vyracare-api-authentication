namespace Vyracare.Auth.Features.Auth.Login;

/// <summary>
/// Define o contrato de entrada ou saída usado por esta feature.
/// </summary>
public sealed record LoginRequest(string Email, string Password);
