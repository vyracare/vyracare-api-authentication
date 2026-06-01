namespace Vyracare.Auth.Features.Auth.Login;

/// <summary>
/// Representa os dados recebidos do cliente para autenticar um usuário.
/// </summary>
/// <param name="Email">E-mail usado para localizar o usuário na base.</param>
/// <param name="Password">Senha em texto puro enviada pelo cliente para validação.</param>
public sealed record LoginRequest(string Email, string Password);
