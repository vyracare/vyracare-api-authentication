using Vyracare.Auth.Common.Results;
using Vyracare.Auth.Common.Time;
using Vyracare.Auth.Features.Auth.Register;
using Vyracare.Auth.Features.Auth.Shared.Domain;
using Vyracare.Auth.Features.Auth.Shared.Ports;

namespace Vyracare.Auth.Tests.Auth.Register;

/// <summary>
/// Agrupa os cen?rios de teste unit?rio relacionados a este componente.
/// </summary>
public sealed class RegisterHandlerTests
{
    [Fact]
/// <summary>
/// Executa a responsabilidade associada a d ev e r et or na r c on fl ic t q ua nd o e ma il j a e xi st ir.
/// </summary>
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
/// <summary>
/// Executa a responsabilidade associada a d ev e c ri ar u su ar io q ua nd o e ma il n ao e xi st ir.
/// </summary>
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
/// <summary>
/// Obt?m ou define u se rs.
/// </summary>
        public List<User> Users { get; } = [];

/// <summary>
/// Persiste um novo registro e devolve a entidade resultante da opera??o.
/// </summary>
        public Task<User> AddAsync(User user)
        {
            user.Id ??= Guid.NewGuid().ToString("N");
            Users.Add(user);
            return Task.FromResult(user);
        }

/// <summary>
/// Recupera um colaborador ou usu?rio a partir do e-mail informado.
/// </summary>
        public Task<User?> GetByEmailAsync(string email)
        {
            return Task.FromResult(Users.FirstOrDefault(user => user.Email == email));
        }

/// <summary>
/// Define a senha inicial do usu?rio quando ainda n?o existe hash cadastrado.
/// </summary>
        public Task<bool> SetPasswordIfEmptyAsync(string email, string passwordHash) => Task.FromResult(false);

/// <summary>
/// Atualiza a senha persistida para o usu?rio informado.
/// </summary>
        public Task<bool> UpdatePasswordAsync(string email, string passwordHash) => Task.FromResult(false);
    }

    private sealed class FakePasswordHasher : IPasswordHasher
    {
/// <summary>
/// Calcula o hash seguro de um valor sens?vel.
/// </summary>
        public string Hash(string password) => $"hash::{password}";

/// <summary>
/// Valida se o valor informado corresponde ao hash persistido.
/// </summary>
        public bool Verify(string password, string storedHash) => Hash(password) == storedHash;
    }

    private sealed class FixedClock : IClock
    {
        public DateTime UtcNow => new(2026, 5, 30, 12, 0, 0, DateTimeKind.Utc);
    }
}
