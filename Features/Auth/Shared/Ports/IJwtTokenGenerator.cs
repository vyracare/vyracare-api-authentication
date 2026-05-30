using Vyracare.Auth.Features.Auth.Shared.Domain;

namespace Vyracare.Auth.Features.Auth.Shared.Ports;

public interface IJwtTokenGenerator
{
    string Generate(User user);
}
