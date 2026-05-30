using Vyracare.Auth.Features.Auth.Shared.Domain;

namespace Vyracare.Auth.Features.Auth.Shared.Ports;

/// <summary>
/// Define um contrato usado para desacoplar a regra de neg?cio dos detalhes de implementa??o.
/// </summary>
public interface IJwtTokenGenerator
{
    string Generate(User user);
}
