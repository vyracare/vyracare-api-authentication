using Vyracare.Auth.Common.Results;
using Vyracare.Auth.Common.Time;
using Vyracare.Auth.Features.Auth.Register;
using Vyracare.Auth.Features.Auth.Shared.Domain;
using Vyracare.Auth.Features.Auth.Shared.Ports;

namespace Vyracare.Auth.Tests.Auth.Register;

/// <summary>
/// Representa o componente RegisterHandlerTests da aplicação.
/// </summary>
public sealed class RegisterHandlerTests
{
    [Fact]
/// <summary>
/// Executa a responsabilidade do método D ev e_r et or na r_c on fl ic t_q ua nd o_e ma il_j a_e xi st ir.
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
/// Executa a responsabilidade do método D ev e_c ri ar_u su ar io_q ua nd o_e ma il_n ao_e xi st ir.
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
/// Obtém ou define o valor da propriedade U se rs.
/// </summary>
        public List<User> Users { get; } = [];

/// <summary>
/// Persiste um novo registro e devolve a entidade resultante da operação.
/// </summary>
        public Task<User> AddAsync(User user)
        {
            user.Id ??= Guid.NewGuid().ToString("N");
            Users.Add(user);
            return Task.FromResult(user);
        }

/// <summary>
/// Recupera um registro específico a partir do e-mail informado.
/// </summary>
        public Task<User?> GetByEmailAsync(string email)
        {
            return Task.FromResult(Users.FirstOrDefault(user => user.Email == email));
        }

/// <summary>
/// Executa a responsabilidade do método S et Pa ss wo rd If Em pt yA sy nc.
/// </summary>
        public Task<bool> SetPasswordIfEmptyAsync(string email, string passwordHash) => Task.FromResult(false);

/// <summary>
/// Atualiza a senha persistida para o usuário informado.
/// </summary>
        public Task<bool> UpdatePasswordAsync(string email, string passwordHash) => Task.FromResult(false);
    }

    private sealed class FakePasswordHasher : IPasswordHasher
    {
/// <summary>
/// Calcula o hash seguro do valor informado.
/// </summary>
        public string Hash(string password) => $"hash::{password}";

/// <summary>
/// Verifica se o valor informado corresponde ao hash armazenado.
/// </summary>
        public bool Verify(string password, string storedHash) => Hash(password) == storedHash;
    }

    private sealed class FixedClock : IClock
    {
        public DateTime UtcNow => new(2026, 5, 30, 12, 0, 0, DateTimeKind.Utc);
    }
}
