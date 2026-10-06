using Vyracare.Auth.Features.Auth.SearchEmployees;
using Vyracare.Auth.Features.Auth.Shared.Domain;
using Vyracare.Auth.Features.Auth.Shared.Ports;

namespace Vyracare.Auth.Tests.Auth.SearchEmployees;

public sealed class SearchEmployeesHandlerTests
{
    [Fact]
    public async Task Deve_retornar_apenas_dados_operacionais_dos_funcionarios_ativos()
    {
        var repository = new FakeUserRepository([
            new User { Id = "1", FullName = "Ana Silva", Email = "ana@vyracare.com", Phone = "11999999999", Role = "Esteticista", Active = true },
            new User { Id = "2", FullName = "Inativo", Email = "inativo@vyracare.com", Active = false }
        ]);

        var result = await new SearchEmployeesHandler(repository).HandleAsync("ana", 10);

        Assert.True(result.IsSuccess);
        var employee = Assert.Single(result.Value!);
        Assert.Equal("1", employee.Id);
        Assert.Equal("Ana Silva", employee.FullName);
        Assert.Equal("ana", repository.Search);
    }

    private sealed class FakeUserRepository(IReadOnlyCollection<User> users) : IUserRepository
    {
        public string? Search { get; private set; }
        public Task<IReadOnlyCollection<User>> SearchActiveAsync(string? search, int limit)
        {
            Search = search;
            return Task.FromResult<IReadOnlyCollection<User>>(users.Where(user => user.Active).Take(limit).ToArray());
        }

        public Task<IReadOnlyCollection<User>> SearchAllAsync(string? search, int limit) => Task.FromResult(users);
        public Task<User?> GetByIdAsync(string id) => Task.FromResult(users.FirstOrDefault(user => user.Id == id));
        public Task<bool> UpdateAsync(User user) => Task.FromResult(true);
        public Task<bool> SetActiveAsync(string id, bool active) => Task.FromResult(true);

        public Task<User?> GetByEmailAsync(string email) => Task.FromResult<User?>(null);
        public Task<User> AddAsync(User user) => Task.FromResult(user);
        public Task<bool> SetPasswordIfEmptyAsync(string email, string passwordHash) => Task.FromResult(false);
        public Task<bool> UpdatePasswordAsync(string email, string passwordHash) => Task.FromResult(false);
    }
}
