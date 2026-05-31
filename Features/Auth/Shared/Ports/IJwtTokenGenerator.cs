using Vyracare.Auth.Features.Auth.Shared.Domain;

namespace Vyracare.Auth.Features.Auth.Shared.Ports;

/// <summary>
/// Define o contrato responsável por gerar tokens de autenticação.
/// </summary>
public interface IJwtTokenGenerator
{
    string Generate(User user);
}
