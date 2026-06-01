using Vyracare.Auth.Features.Auth.Shared.Domain;

namespace Vyracare.Auth.Features.Auth.Shared.Ports;

/// <summary>
/// Define a porta responsável por emitir tokens JWT a partir dos dados de um usuário autenticado.
/// </summary>
public interface IJwtTokenGenerator
{
    /// <summary>
    /// Gera um token de acesso para o usuário informado.
    /// </summary>
    /// <param name="user">Usuário autenticado que servirá de base para as claims do token.</param>
    /// <returns>Token JWT serializado em formato texto.</returns>
    string Generate(User user);
}
