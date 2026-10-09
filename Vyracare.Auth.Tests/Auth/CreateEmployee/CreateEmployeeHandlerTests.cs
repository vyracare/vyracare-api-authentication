using Vyracare.Auth.Common.Time;
using Vyracare.Auth.Features.Auth.CreateEmployee;
using Vyracare.Auth.Features.Auth.Register;
using Vyracare.Auth.Features.Auth.Shared.Domain;
using Vyracare.Auth.Features.Auth.Shared.Ports;

namespace Vyracare.Auth.Tests.Auth.CreateEmployee;

public sealed class CreateEmployeeHandlerTests
{
    [Fact]
    public async Task Creates_administrator_membership_in_requester_tenant()
    {
        var repository = new Repository();
        var tenancy = new Tenancy();
        var handler = new CreateEmployeeHandler(repository, new Passwords(), new Clock(), tenancy);

        var result = await handler.HandleAsync("tenant-a", new RegisterRequest(
            "employee@example.com", null, "Employee", null, null, null, "Administrador", true));

        Assert.True(result.IsSuccess);
        Assert.Equal("tenant-a", tenancy.TenantId);
        Assert.Equal("Administrator", tenancy.Role);
        Assert.Equal("tenant-a", repository.Created?.TenantAccess?.TenantId);
    }

    private sealed class Repository : IUserRepository
    {
        public User? Created { get; private set; }
        public Task<User?> GetByEmailAsync(string email) => Task.FromResult<User?>(null);
        public Task<User> AddAsync(User user) { user.Id = "user-a"; Created = user; return Task.FromResult(user); }
        public Task<bool> SetTenantAccessAsync(string id, TenantAccess access) => Task.FromResult(true);
        public Task<bool> DeleteAsync(string id) => Task.FromResult(true);
        public Task<IReadOnlyCollection<User>> SearchActiveAsync(string? search, int limit) => Task.FromResult<IReadOnlyCollection<User>>([]);
        public Task<IReadOnlyCollection<User>> SearchAllAsync(string? search, int limit) => Task.FromResult<IReadOnlyCollection<User>>([]);
        public Task<User?> GetByIdAsync(string id) => Task.FromResult<User?>(null);
        public Task<bool> UpdateAsync(User user) => Task.FromResult(false);
        public Task<bool> SetActiveAsync(string id, bool active) => Task.FromResult(false);
        public Task<bool> SetPasswordIfEmptyAsync(string email, string passwordHash) => Task.FromResult(false);
        public Task<bool> UpdatePasswordAsync(string email, string passwordHash) => Task.FromResult(false);
    }

    private sealed class Tenancy : ITenancyProvisioner
    {
        public string? TenantId { get; private set; }
        public string? Role { get; private set; }
        public Task<TenantAccess> ProvisionMemberAsync(string tenantId, string userId, string role)
        {
            TenantId = tenantId;
            Role = role;
            return Task.FromResult(new TenantAccess(tenantId, "membership-a", role, "Trialing", DateTime.UtcNow, DateTime.UtcNow.AddDays(30)));
        }
        public Task<TenantAccess> ProvisionOwnerAsync(string userId, OrganizationRegistration organization, string idempotencyKey) => throw new NotSupportedException();
        public Task CompensateOwnerAsync(string tenantId, string userId) => Task.CompletedTask;
    }

    private sealed class Passwords : IPasswordHasher
    {
        public string Hash(string password) => "hash";
        public bool Verify(string password, string storedHash) => false;
    }

    private sealed class Clock : IClock { public DateTime UtcNow => new(2026, 10, 9, 0, 0, 0, DateTimeKind.Utc); }
}
