namespace Vyracare.Auth.Features.Auth.ForgotPassword;

/// <summary>
/// Define o contrato de entrada ou saída usado por esta feature.
/// </summary>
public sealed record ForgotPasswordRequest(string Email, string Password);
