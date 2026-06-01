namespace Vyracare.Auth.Features.Auth.Shared;

/// <summary>
/// Representa uma resposta simples baseada apenas em mensagem textual.
/// É usada em fluxos que não precisam devolver payload rico além da confirmação da operação.
/// </summary>
/// <param name="Message">Mensagem descritiva do resultado da operação.</param>
public sealed record MessageResponse(string Message);
