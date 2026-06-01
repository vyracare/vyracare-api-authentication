using Vyracare.Auth.Common.Results;
using Vyracare.Auth.Features.Auth.Login;
using Vyracare.Auth.Features.Auth.Shared.Domain;
using Vyracare.Auth.Features.Auth.Shared.Ports;

namespace Vyracare.Auth.Tests.Auth.Login;

/// <summary>
/// Agrupa os testes unitários do caso de uso de login.
/// Aqui o objetivo é validar a regra de autenticação sem depender de MongoDB, JWT real ou HTTP.
/// </summary>
public sealed class LoginHandlerTests
{
    [Fact]
    /// <summary>
    /// Garante que o login falha com status de não autorizado quando o e-mail informado
    /// não corresponde a nenhum usuário persistido.
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
    /// Garante que o handler devolve token quando o usuário existe e a senha informada
    /// corresponde ao hash persistido.
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

    /// <summary>
    /// Fake em memória usado para simular persistência nos testes do handler.
    /// </summary>
    private sealed class FakeUserRepository : IUserRepository
    {
        private readonly List<User> _users = [];

        /// <summary>
        /// Adiciona um usuário à lista local e simula a geração de identificador.
        /// </summary>
        public Task<User> AddAsync(User user)
        {
            user.Id ??= Guid.NewGuid().ToString("N");
            _users.Add(user);
            return Task.FromResult(user);
        }

        /// <summary>
        /// Localiza um usuário pelo e-mail dentro da coleção em memória.
        /// </summary>
        public Task<User?> GetByEmailAsync(string email)
        {
            return Task.FromResult(_users.FirstOrDefault(user => user.Email == email));
        }

        /// <summary>
        /// Não é usado por estes testes; retorna sempre falso.
        /// </summary>
        public Task<bool> SetPasswordIfEmptyAsync(string email, string passwordHash) => Task.FromResult(false);

        /// <summary>
        /// Não é usado por estes testes; retorna sempre falso.
        /// </summary>
        public Task<bool> UpdatePasswordAsync(string email, string passwordHash) => Task.FromResult(false);
    }

    /// <summary>
    /// Fake de hashing que torna os testes determinísticos.
    /// </summary>
    private sealed class FakePasswordHasher : IPasswordHasher
    {
        /// <summary>
        /// Devolve sempre o mesmo hash esperado pelos testes.
        /// </summary>
        public string Hash(string password) => "hash-123";

        /// <summary>
        /// Considera válida apenas a combinação esperada pelo cenário de teste.
        /// </summary>
        public bool Verify(string password, string storedHash) => password == "123456" && storedHash == "hash-123";
    }

    /// <summary>
    /// Fake do gerador de token que evita dependência de criptografia real nos testes.
    /// </summary>
    private sealed class FakeJwtTokenGenerator : IJwtTokenGenerator
    {
        /// <summary>
        /// Devolve um token fixo para facilitar a asserção do cenário.
        /// </summary>
        public string Generate(User user) => "token-fake";
    }
}
