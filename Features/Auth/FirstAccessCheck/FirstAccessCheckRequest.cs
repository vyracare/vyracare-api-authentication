namespace Vyracare.Auth.Features.Auth.FirstAccessCheck;

/// <summary>
/// Representa a entrada do fluxo que verifica se o usuário existe e se ainda pode definir a senha inicial.
/// </summary>
/// <param name="Email">E-mail do usuário que será consultado.</param>
public sealed record FirstAccessCheckRequest(string Email);
