using Vyracare.Auth.Common.Results;
using Vyracare.Auth.Features.Auth.ManageEmployees;
using Vyracare.Auth.Features.Auth.Shared.Domain;
using Vyracare.Auth.Features.Auth.Shared.Ports;

namespace Vyracare.Auth.Tests.Auth.ManageEmployees;

public sealed class ManageEmployeesHandlerTests
{
    [Fact]
    public async Task Deve_listar_ativos_e_inativos_sem_credenciais()
    {
        var repository = new Repository();
        var result = await new ListManagedEmployeesHandler(repository).HandleAsync(null, 100);

        Assert.True(result.IsSuccess);
        Assert.Equal(2, result.Value!.Count);
        Assert.Contains(result.Value, employee => !employee.Active);
    }

    [Fact]
    public async Task Deve_editar_dados_sem_alterar_senha()
    {
        var repository = new Repository();
        var result = await new UpdateEmployeeHandler(repository).HandleAsync("1", new UpdateEmployeeRequest(
            "Nome atualizado", "novo@vyracare.com", "11999999999", "Clinico", "Saude", "Gestor", true), "admin-2");

        Assert.True(result.IsSuccess);
        Assert.Equal("Nome atualizado", repository.Users[0].FullName);
        Assert.Equal("hash-original", repository.Users[0].PasswordHash);
    }

    [Fact]
    public async Task Deve_impedir_autoinativacao()
    {
        var repository = new Repository();
        var result = await new ChangeEmployeeStatusHandler(repository)
            .HandleAsync("1", new ChangeEmployeeStatusRequest(false), "1");

        Assert.False(result.IsSuccess);
        Assert.Equal(UseCaseErrorType.Validation, result.ErrorType);
        Assert.True(repository.Users[0].Active);
    }

    [Fact]
    public async Task Deve_impedir_autoinativacao_tambem_na_edicao_completa()
    {
        var repository = new Repository();
        var result = await new UpdateEmployeeHandler(repository).HandleAsync("1", new UpdateEmployeeRequest(
            "Admin", "admin@vyracare.com", null, null, null, "Administrador", false), "1");

        Assert.False(result.IsSuccess);
        Assert.Equal(UseCaseErrorType.Validation, result.ErrorType);
        Assert.True(repository.Users[0].Active);
    }

    private sealed class Repository : IUserRepository
    {
        public List<User> Users { get; } =
        [
            new() { Id = "1", FullName = "Admin", Email = "admin@vyracare.com", PasswordHash = "hash-original", Active = true },
            new() { Id = "2", FullName = "Inativo", Email = "inativo@vyracare.com", Active = false }
        ];

        public Task<User?> GetByEmailAsync(string email) => Task.FromResult(Users.FirstOrDefault(user => user.Email == email));
        public Task<User?> GetByIdAsync(string id) => Task.FromResult(Users.FirstOrDefault(user => user.Id == id));
        public Task<IReadOnlyCollection<User>> SearchActiveAsync(string? search, int limit) => Task.FromResult<IReadOnlyCollection<User>>(Users.Where(user => user.Active).Take(limit).ToArray());
        public Task<IReadOnlyCollection<User>> SearchAllAsync(string? search, int limit) => Task.FromResult<IReadOnlyCollection<User>>(Users.Take(limit).ToArray());
        public Task<User> AddAsync(User user) => Task.FromResult(user);
        public Task<bool> UpdateAsync(User user) => Task.FromResult(true);
        public Task<bool> SetActiveAsync(string id, bool active)
        {
            var user = Users.FirstOrDefault(item => item.Id == id);
            if (user is null) return Task.FromResult(false);
            user.Active = active;
            return Task.FromResult(true);
        }
        public Task<bool> SetPasswordIfEmptyAsync(string email, string passwordHash) => Task.FromResult(false);
        public Task<bool> UpdatePasswordAsync(string email, string passwordHash) => Task.FromResult(false);
    }
}
