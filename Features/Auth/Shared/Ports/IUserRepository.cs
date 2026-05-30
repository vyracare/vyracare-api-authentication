using Vyracare.Auth.Features.Auth.Shared.Domain;

namespace Vyracare.Auth.Features.Auth.Shared.Ports;

public interface IUserRepository
{
    Task<User?> GetByEmailAsync(string email);
    Task<User> AddAsync(User user);
    Task<bool> SetPasswordIfEmptyAsync(string email, string passwordHash);
    Task<bool> UpdatePasswordAsync(string email, string passwordHash);
}
