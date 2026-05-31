using Vyracare.Auth.Common.Results;
using Vyracare.Auth.Features.Auth.Login;
using Vyracare.Auth.Features.Auth.Shared.Domain;
using Vyracare.Auth.Features.Auth.Shared.Ports;

namespace Vyracare.Auth.Tests.Auth.Login;

/// <summary>
/// Representa o componente LoginHandlerTests da aplicação.
/// </summary>
public sealed class LoginHandlerTests
{
    [Fact]
/// <summary>
/// Executa a responsabilidade do método D ev e_r et or na r_u na ut ho ri ze d_q ua nd o_u su ar io_n ao_e xi st ir.
/// </summary>
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
/// <summary>
/// Executa a responsabilidade do método D ev e_r et or na r_t ok en_q ua nd o_c re de nc ia is_f or e_v al id as.
/// </summary>
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

/// <summary>
/// Persiste um novo registro e devolve a entidade resultante da operação.
/// </summary>
        public Task<User> AddAsync(User user)
        {
            user.Id ??= Guid.NewGuid().ToString("N");
            _users.Add(user);
            return Task.FromResult(user);
        }

/// <summary>
/// Recupera um registro específico a partir do e-mail informado.
/// </summary>
        public Task<User?> GetByEmailAsync(string email)
        {
            return Task.FromResult(_users.FirstOrDefault(user => user.Email == email));
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
        public string Hash(string password) => "hash-123";

/// <summary>
/// Verifica se o valor informado corresponde ao hash armazenado.
/// </summary>
        public bool Verify(string password, string storedHash) => password == "123456" && storedHash == "hash-123";
    }

    private sealed class FakeJwtTokenGenerator : IJwtTokenGenerator
    {
/// <summary>
/// Gera um token a partir das informações do usuário informado.
/// </summary>
        public string Generate(User user) => "token-fake";
    }
}
