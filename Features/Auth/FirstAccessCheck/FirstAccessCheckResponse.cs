namespace Vyracare.Auth.Features.Auth.FirstAccessCheck;

/// <summary>
/// Define o contrato de entrada ou saída usado por esta feature.
/// </summary>
public sealed record FirstAccessCheckResponse(bool Exists, bool CanSetPassword);
