using Vyracare.Auth.Common.Time;
using Vyracare.Auth.Features.Auth;
using Vyracare.Auth.Features.Auth.Register;
using Vyracare.Auth.Features.Auth.Shared.Domain;
using Vyracare.Auth.Features.Auth.Shared.Ports;

namespace Vyracare.Auth.Tests.Auth.Register;

public sealed class PublicRegistrationSecurityTests
{
    [Fact]
    public async Task Deve_ignorar_campos_privilegiados_no_registro_publico()
    {
        var repository = new Repository();
        var handler = new RegisterHandler(repository, new Hasher(), new Clock(), new TenancyProvisioner(), new TokenGenerator());
        var controller = new AuthController();
        var request = new RegisterRequest(
            "publico@vyracare.com", "senha", "Publico", "Administrador", "TI", "11999999999", "Administrador", false,
            new OrganizationRegistration("Clinica Publica", null, null));

        await controller.Register(request, handler);

        var created = Assert.Single(repository.Users);
        Assert.Equal("Administrador", created.AccessLevel);
        Assert.True(created.Active);
        Assert.Null(created.Role);
        Assert.Null(created.Department);
        Assert.Null(created.Phone);
    }

    private sealed class Repository : IUserRepository
    {
        public List<User> Users { get; } = [];
        public Task<User?> GetByEmailAsync(string email) => Task.FromResult(Users.FirstOrDefault(user => user.Email == email));
        public Task<User?> GetByIdAsync(string id) => Task.FromResult(Users.FirstOrDefault(user => user.Id == id));
        public Task<IReadOnlyCollection<User>> SearchActiveAsync(string? search, int limit) => Task.FromResult<IReadOnlyCollection<User>>([]);
        public Task<IReadOnlyCollection<User>> SearchAllAsync(string? search, int limit) => Task.FromResult<IReadOnlyCollection<User>>(Users);
        public Task<User> AddAsync(User user)
        {
            user.Id = "created-id";
            Users.Add(user);
            return Task.FromResult(user);
        }
        public Task<bool> UpdateAsync(User user) => Task.FromResult(true);
        public Task<bool> SetActiveAsync(string id, bool active) => Task.FromResult(true);
        public Task<bool> DeleteAsync(string id) => Task.FromResult(Users.RemoveAll(user => user.Id == id) > 0);
        public Task<bool> SetPasswordIfEmptyAsync(string email, string passwordHash) => Task.FromResult(false);
        public Task<bool> UpdatePasswordAsync(string email, string passwordHash) => Task.FromResult(false);
    }

    private sealed class Hasher : IPasswordHasher
    {
        public string Hash(string password) => "hash";
        public bool Verify(string password, string storedHash) => true;
    }

    private sealed class Clock : IClock
    {
        public DateTime UtcNow => new(2026, 10, 6, 0, 0, 0, DateTimeKind.Utc);
    }

    private sealed class TenancyProvisioner : ITenancyProvisioner
    {
        public Task<TenantAccess> ProvisionOwnerAsync(string userId, OrganizationRegistration organization, string idempotencyKey) =>
            Task.FromResult(new TenantAccess("tenant-a", "membership-a", "Owner", "Trialing", DateTime.UtcNow, DateTime.UtcNow.AddDays(30)));
        public Task CompensateOwnerAsync(string tenantId, string userId) => Task.CompletedTask;
    }

    private sealed class TokenGenerator : IJwtTokenGenerator
    {
        public string Generate(User user) => "token";
    }
}
