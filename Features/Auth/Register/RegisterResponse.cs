namespace Vyracare.Auth.Features.Auth.Register;

/// <summary>
/// Representa a resposta de sucesso do cadastro.
/// </summary>
/// <param name="Id">Identificador do usuário criado no repositório.</param>
/// <param name="Message">Mensagem simples usada para confirmar a criação ao cliente.</param>
public sealed record RegisterResponse(string Id, string Message);
