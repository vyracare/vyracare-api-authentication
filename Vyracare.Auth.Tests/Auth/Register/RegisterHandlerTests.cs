using Vyracare.Auth.Common.Results;
using Vyracare.Auth.Common.Time;
using Vyracare.Auth.Features.Auth.Register;
using Vyracare.Auth.Features.Auth.Shared.Domain;
using Vyracare.Auth.Features.Auth.Shared.Ports;

namespace Vyracare.Auth.Tests.Auth.Register;

public sealed class RegisterHandlerTests
{
    [Fact]
    public async Task Deve_retornar_conflict_quando_email_ja_existir()
    {
        var repository = new FakeUserRepository();
        await repository.AddAsync(new User { Email = "duplicado@vyracare.com", PasswordHash = "hash" });

        var handler = new RegisterHandler(repository, new FakePasswordHasher(), new FixedClock());

        var result = await handler.HandleAsync(new RegisterRequest("duplicado@vyracare.com", "123456", "Nome", null, null, null, null, true));

        Assert.False(result.IsSuccess);
        Assert.Equal(UseCaseErrorType.Conflict, result.ErrorType);
    }

    [Fact]
    public async Task Deve_criar_usuario_quando_email_nao_existir()
    {
        var repository = new FakeUserRepository();
        var handler = new RegisterHandler(repository, new FakePasswordHasher(), new FixedClock());

        var result = await handler.HandleAsync(new RegisterRequest("novo@vyracare.com", "123456", "Novo", "Admin", "TI", "11999999999", "total", true));

        Assert.True(result.IsSuccess);
        Assert.Equal("User created", result.Value!.Message);
        Assert.Single(repository.Users);
    }

    private sealed class FakeUserRepository : IUserRepository
    {
        public List<User> Users { get; } = [];

        public Task<User> AddAsync(User user)
        {
            user.Id ??= Guid.NewGuid().ToString("N");
            Users.Add(user);
            return Task.FromResult(user);
        }

        public Task<User?> GetByEmailAsync(string email)
        {
            return Task.FromResult(Users.FirstOrDefault(user => user.Email == email));
        }

        public Task<bool> SetPasswordIfEmptyAsync(string email, string passwordHash) => Task.FromResult(false);

        public Task<bool> UpdatePasswordAsync(string email, string passwordHash) => Task.FromResult(false);
    }

    private sealed class FakePasswordHasher : IPasswordHasher
    {
        public string Hash(string password) => $"hash::{password}";

        public bool Verify(string password, string storedHash) => Hash(password) == storedHash;
    }

    private sealed class FixedClock : IClock
    {
        public DateTime UtcNow => new(2026, 5, 30, 12, 0, 0, DateTimeKind.Utc);
    }
}
