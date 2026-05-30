using Vyracare.Auth.Common.Results;
using Vyracare.Auth.Features.Auth.Login;
using Vyracare.Auth.Features.Auth.Shared.Domain;
using Vyracare.Auth.Features.Auth.Shared.Ports;

namespace Vyracare.Auth.Tests.Auth.Login;

/// <summary>
/// Agrupa os cen?rios de teste unit?rio relacionados a este componente.
/// </summary>
public sealed class LoginHandlerTests
{
    [Fact]
/// <summary>
/// Executa a responsabilidade associada a d ev e r et or na r u na ut ho ri ze d q ua nd o u su ar io n ao e xi st ir.
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
/// Executa a responsabilidade associada a d ev e r et or na r t ok en q ua nd o c re de nc ia is f or e v al id as.
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
/// Persiste um novo registro e devolve a entidade resultante da opera??o.
/// </summary>
        public Task<User> AddAsync(User user)
        {
            user.Id ??= Guid.NewGuid().ToString("N");
            _users.Add(user);
            return Task.FromResult(user);
        }

/// <summary>
/// Recupera um colaborador ou usu?rio a partir do e-mail informado.
/// </summary>
        public Task<User?> GetByEmailAsync(string email)
        {
            return Task.FromResult(_users.FirstOrDefault(user => user.Email == email));
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
        public string Hash(string password) => "hash-123";

/// <summary>
/// Valida se o valor informado corresponde ao hash persistido.
/// </summary>
        public bool Verify(string password, string storedHash) => password == "123456" && storedHash == "hash-123";
    }

    private sealed class FakeJwtTokenGenerator : IJwtTokenGenerator
    {
/// <summary>
/// Gera um valor derivado a partir do estado informado, como um token de autentica??o.
/// </summary>
        public string Generate(User user) => "token-fake";
    }
}
