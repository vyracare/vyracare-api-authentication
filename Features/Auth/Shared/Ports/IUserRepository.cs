using Vyracare.Auth.Features.Auth.Shared.Domain;

namespace Vyracare.Auth.Features.Auth.Shared.Ports;

/// <summary>
/// Implementa a integra??o com a persist?ncia ou com uma depend?ncia externa da aplica??o.
/// </summary>
public interface IUserRepository
{
    Task<User?> GetByEmailAsync(string email);
    Task<User> AddAsync(User user);
    Task<bool> SetPasswordIfEmptyAsync(string email, string passwordHash);
    Task<bool> UpdatePasswordAsync(string email, string passwordHash);
}
