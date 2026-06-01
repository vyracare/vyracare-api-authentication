namespace Vyracare.Auth.Features.Auth.FirstAccessCheck;

/// <summary>
/// Representa a resposta do fluxo de verificação de primeiro acesso.
/// </summary>
/// <param name="Exists">Indica se o e-mail informado corresponde a um usuário existente.</param>
/// <param name="CanSetPassword">Indica se o usuário pode definir a senha inicial neste momento.</param>
public sealed record FirstAccessCheckResponse(bool Exists, bool CanSetPassword);
