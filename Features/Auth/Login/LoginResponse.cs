namespace Vyracare.Auth.Features.Auth.Login;

/// <summary>
/// Representa a resposta de sucesso do fluxo de login.
/// </summary>
/// <param name="Token">JWT gerado para o usuário autenticado.</param>
public sealed record LoginResponse(string Token);
