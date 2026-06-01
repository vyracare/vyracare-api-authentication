namespace Vyracare.Auth.Features.Auth.ForgotPassword;

/// <summary>
/// Representa os dados necessários para redefinir a senha de um usuário existente.
/// </summary>
/// <param name="Email">E-mail do usuário cuja senha será atualizada.</param>
/// <param name="Password">Nova senha escolhida no fluxo de recuperação.</param>
public sealed record ForgotPasswordRequest(string Email, string Password);
