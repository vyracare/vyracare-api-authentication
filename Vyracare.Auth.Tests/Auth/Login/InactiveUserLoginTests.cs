using Vyracare.Auth.Common.Results;
using Vyracare.Auth.Features.Auth.Login;
using Vyracare.Auth.Features.Auth.Shared.Domain;
using Vyracare.Auth.Features.Auth.Shared.Ports;

namespace Vyracare.Auth.Tests.Auth.Login;

public sealed class InactiveUserLoginTests
{
    [Fact]
    public async Task Deve_negar_login_de_usuario_inativo()
    {
        var handler = new LoginHandler(new Repository(), new Hasher(), new TokenGenerator());

        var result = await handler.HandleAsync(new LoginRequest("inativo@vyracare.com", "senha"));

        Assert.False(result.IsSuccess);
        Assert.Equal(UseCaseErrorType.Unauthorized, result.ErrorType);
    }

    private sealed class Repository : IUserRepository
    {
        private readonly User user = new() { Id = "1", Email = "inativo@vyracare.com", PasswordHash = "hash", Active = false };
        public Task<User?> GetByEmailAsync(string email) => Task.FromResult<User?>(user);
        public Task<User?> GetByIdAsync(string id) => Task.FromResult<User?>(user);
        public Task<IReadOnlyCollection<User>> SearchActiveAsync(string? search, int limit) => Task.FromResult<IReadOnlyCollection<User>>([]);
        public Task<IReadOnlyCollection<User>> SearchAllAsync(string? search, int limit) => Task.FromResult<IReadOnlyCollection<User>>([user]);
        public Task<User> AddAsync(User value) => Task.FromResult(value);
        public Task<bool> UpdateAsync(User value) => Task.FromResult(true);
        public Task<bool> SetActiveAsync(string id, bool active) => Task.FromResult(true);
        public Task<bool> DeleteAsync(string id) => Task.FromResult(false);
        public Task<bool> SetPasswordIfEmptyAsync(string email, string passwordHash) => Task.FromResult(false);
        public Task<bool> UpdatePasswordAsync(string email, string passwordHash) => Task.FromResult(false);
    }

    private sealed class Hasher : IPasswordHasher
    {
        public string Hash(string password) => "hash";
        public bool Verify(string password, string storedHash) => true;
    }

    private sealed class TokenGenerator : IJwtTokenGenerator
    {
        public string Generate(User user) => "token";
    }
}
