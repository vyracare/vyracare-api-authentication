namespace Vyracare.Auth.Features.Auth.FirstAccessSetPassword;

/// <summary>
/// Representa a entrada usada para definir a senha inicial de um usuário.
/// </summary>
/// <param name="Email">E-mail do usuário que está concluindo o primeiro acesso.</param>
/// <param name="Password">Senha inicial escolhida pelo usuário.</param>
public sealed record FirstAccessSetPasswordRequest(string Email, string Password);
