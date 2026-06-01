using Vyracare.Auth.Common.Results;
using Vyracare.Auth.Common.Time;
using Vyracare.Auth.Features.Auth.Register;
using Vyracare.Auth.Features.Auth.Shared.Domain;
using Vyracare.Auth.Features.Auth.Shared.Ports;

namespace Vyracare.Auth.Tests.Auth.Register;

/// <summary>
/// Agrupa os testes unitários do caso de uso de registro de usuário.
/// Os cenários aqui validam a regra de negócio sem depender de banco, relógio real ou infraestrutura externa.
/// </summary>
public sealed class RegisterHandlerTests
{
    [Fact]
    /// <summary>
    /// Garante que o cadastro devolve conflito quando já existe usuário com o mesmo e-mail.
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
    /// Garante que o cadastro cria um novo usuário quando o e-mail ainda não está em uso.
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

    /// <summary>
    /// Fake em memória que simula o comportamento do repositório de usuários durante os testes.
    /// </summary>
    private sealed class FakeUserRepository : IUserRepository
    {
        /// <summary>
        /// Lista usada para armazenar os usuários gravados durante a execução dos testes.
        /// </summary>
        public List<User> Users { get; } = [];

        /// <summary>
        /// Adiciona um usuário à coleção em memória e simula a geração de identificador.
        /// </summary>
        public Task<User> AddAsync(User user)
        {
            user.Id ??= Guid.NewGuid().ToString("N");
            Users.Add(user);
            return Task.FromResult(user);
        }

        /// <summary>
        /// Busca um usuário pelo e-mail na coleção em memória.
        /// </summary>
        public Task<User?> GetByEmailAsync(string email)
        {
            return Task.FromResult(Users.FirstOrDefault(user => user.Email == email));
        }

        /// <summary>
        /// Não participa destes cenários de teste; retorna falso por padrão.
        /// </summary>
        public Task<bool> SetPasswordIfEmptyAsync(string email, string passwordHash) => Task.FromResult(false);

        /// <summary>
        /// Não participa destes cenários de teste; retorna falso por padrão.
        /// </summary>
        public Task<bool> UpdatePasswordAsync(string email, string passwordHash) => Task.FromResult(false);
    }

    /// <summary>
    /// Fake do hasher usada para deixar o valor do hash previsível durante os testes.
    /// </summary>
    private sealed class FakePasswordHasher : IPasswordHasher
    {
        /// <summary>
        /// Gera um hash textual simples o suficiente para ser validado nas asserções.
        /// </summary>
        public string Hash(string password) => $"hash::{password}";

        /// <summary>
        /// Compara a senha recebida com o formato de hash simplificado usado nos testes.
        /// </summary>
        public bool Verify(string password, string storedHash) => Hash(password) == storedHash;
    }

    /// <summary>
    /// Fake de relógio que fixa a data de criação do usuário para evitar dependência do relógio real.
    /// </summary>
    private sealed class FixedClock : IClock
    {
        /// <summary>
        /// Obtém a data fixa usada pelos cenários de teste.
        /// </summary>
        public DateTime UtcNow => new(2026, 5, 30, 12, 0, 0, DateTimeKind.Utc);
    }
}
