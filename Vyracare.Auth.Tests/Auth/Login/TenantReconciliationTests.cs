using Vyracare.Auth.Features.Auth.Login;
using Vyracare.Auth.Features.Auth.Register;
using Vyracare.Auth.Features.Auth.Shared.Domain;
using Vyracare.Auth.Features.Auth.Shared.Ports;

namespace Vyracare.Auth.Tests.Auth.Login;

public sealed class TenantReconciliationTests
{
    [Fact]
    public async Task Restores_single_active_membership_before_issuing_token()
    {
        var user = new User { Id = "user-a", Email = "employee@example.com", PasswordHash = "hash", Active = true };
        var repository = new Repository(user);
        var handler = new LoginHandler(repository, new Passwords(), new Tokens(), new Tenancy());

        var result = await handler.HandleAsync(new LoginRequest(user.Email, "password"));

        Assert.True(result.IsSuccess);
        Assert.Equal("tenant-a", user.TenantAccess?.TenantId);
        Assert.True(repository.ProjectionPersisted);
    }

    private sealed class Repository(User user) : IUserRepository
    {
        public bool ProjectionPersisted { get; private set; }
        public Task<User?> GetByEmailAsync(string email) => Task.FromResult<User?>(email == user.Email ? user : null);
        public Task<bool> SetTenantAccessAsync(string id, TenantAccess access) { ProjectionPersisted = true; return Task.FromResult(true); }
        public Task<IReadOnlyCollection<User>> SearchActiveAsync(string? search, int limit) => Task.FromResult<IReadOnlyCollection<User>>([]);
        public Task<IReadOnlyCollection<User>> SearchAllAsync(string? search, int limit) => Task.FromResult<IReadOnlyCollection<User>>([]);
        public Task<User?> GetByIdAsync(string id) => Task.FromResult<User?>(null);
        public Task<bool> UpdateAsync(User value) => Task.FromResult(false);
        public Task<bool> SetActiveAsync(string id, bool active) => Task.FromResult(false);
        public Task<bool> DeleteAsync(string id) => Task.FromResult(false);
        public Task<User> AddAsync(User value) => Task.FromResult(value);
        public Task<bool> SetPasswordIfEmptyAsync(string email, string passwordHash) => Task.FromResult(false);
        public Task<bool> UpdatePasswordAsync(string email, string passwordHash) => Task.FromResult(false);
    }

    private sealed class Tenancy : ITenancyProvisioner
    {
        public Task<IReadOnlyCollection<TenantAccess>> GetMembershipsAsync(string userId) =>
            Task.FromResult<IReadOnlyCollection<TenantAccess>>([new TenantAccess(
                "tenant-a", "membership-a", "Administrator", "Trialing", DateTime.UtcNow, DateTime.UtcNow.AddDays(30))]);
        public Task<TenantAccess> ProvisionOwnerAsync(string userId, OrganizationRegistration organization, string idempotencyKey) => throw new NotSupportedException();
        public Task CompensateOwnerAsync(string tenantId, string userId) => Task.CompletedTask;
    }

    private sealed class Passwords : IPasswordHasher
    {
        public string Hash(string password) => "hash";
        public bool Verify(string password, string storedHash) => password == "password" && storedHash == "hash";
    }

    private sealed class Tokens : IJwtTokenGenerator { public string Generate(User user) => "token"; }
}
