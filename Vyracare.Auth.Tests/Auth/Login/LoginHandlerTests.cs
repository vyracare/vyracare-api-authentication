using Vyracare.Auth.Common.Results;
using Vyracare.Auth.Features.Auth.Login;
using Vyracare.Auth.Features.Auth.Shared.Domain;
using Vyracare.Auth.Features.Auth.Shared.Ports;

namespace Vyracare.Auth.Tests.Auth.Login;

public sealed class LoginHandlerTests
{
    [Fact]
    public async Task Deve_retornar_unauthorized_quando_usuario_nao_existir()
    {
        var handler = new LoginHandler(
            new FakeUserRepository(),
            new FakePasswordHasher(),
            new FakeJwtTokenGenerator());

        var result = await handler.HandleAsync(new LoginRequest("nao-existe@vyracare.com", "123456"));

        Assert.False(result.IsSuccess);
        Assert.Equal(UseCaseErrorType.Unauthorized, result.ErrorType);
    }

    [Fact]
    public async Task Deve_retornar_token_quando_credenciais_fore_validas()
    {
        var repository = new FakeUserRepository();
        await repository.AddAsync(new User
        {
            Id = "user-1",
            Email = "lenin@vyracare.com",
            PasswordHash = "hash-123"
        });

        var handler = new LoginHandler(
            repository,
            new FakePasswordHasher(),
            new FakeJwtTokenGenerator());

        var result = await handler.HandleAsync(new LoginRequest("lenin@vyracare.com", "123456"));

        Assert.True(result.IsSuccess);
        Assert.Equal("token-fake", result.Value!.Token);
    }

    private sealed class FakeUserRepository : IUserRepository
    {
        private readonly List<User> _users = [];

        public Task<User> AddAsync(User user)
        {
            user.Id ??= Guid.NewGuid().ToString("N");
            _users.Add(user);
            return Task.FromResult(user);
        }

        public Task<User?> GetByEmailAsync(string email)
        {
            return Task.FromResult(_users.FirstOrDefault(user => user.Email == email));
        }

        public Task<bool> SetPasswordIfEmptyAsync(string email, string passwordHash) => Task.FromResult(false);

        public Task<bool> UpdatePasswordAsync(string email, string passwordHash) => Task.FromResult(false);
    }

    private sealed class FakePasswordHasher : IPasswordHasher
    {
        public string Hash(string password) => "hash-123";

        public bool Verify(string password, string storedHash) => password == "123456" && storedHash == "hash-123";
    }

    private sealed class FakeJwtTokenGenerator : IJwtTokenGenerator
    {
        public string Generate(User user) => "token-fake";
    }
}
