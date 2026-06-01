using Vyracare.Auth.Features.Auth.Shared.Domain;

namespace Vyracare.Auth.Features.Auth.Shared.Ports;

/// <summary>
/// Define o contrato de persistência usado pela feature.
/// </summary>
public interface IUserRepository
{
    Task<User?> GetByEmailAsync(string email);
    Task<User> AddAsync(User user);
    Task<bool> SetPasswordIfEmptyAsync(string email, string passwordHash);
    Task<bool> UpdatePasswordAsync(string email, string passwordHash);
}
