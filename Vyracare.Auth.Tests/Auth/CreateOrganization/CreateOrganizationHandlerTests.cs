using Vyracare.Auth.Features.Auth.CreateOrganization;
using Vyracare.Auth.Features.Auth.Register;
using Vyracare.Auth.Features.Auth.Shared.Domain;
using Vyracare.Auth.Features.Auth.Shared.Ports;

namespace Vyracare.Auth.Tests.Auth.CreateOrganization;

public sealed class CreateOrganizationHandlerTests
{
    [Fact]
    public async Task Promotes_existing_user_to_owner_and_administrator()
    {
        var user = new User { Id = "user-a", Email = "user@example.com", Active = true, AccessLevel = "Leitura" };
        var repository = new Repository(user);
        var handler = new CreateOrganizationHandler(repository, new Tenancy(), new Tokens());

        var result = await handler.HandleAsync("user-a", new CreateOrganizationRequest("Clinica A", null, null));

        Assert.True(result.IsSuccess);
        Assert.Equal("Administrador", user.AccessLevel);
        Assert.Equal("Owner", user.TenantAccess?.Role);
        Assert.Equal("tenant-token", result.Value?.Token);
    }

    private sealed class Repository(User user) : IUserRepository
    {
        public Task<User?> GetByIdAsync(string id) => Task.FromResult<User?>(id == user.Id ? user : null);
        public Task<bool> UpdateAsync(User value) => Task.FromResult(true);
        public Task<bool> SetTenantAccessAsync(string id, TenantAccess access) => Task.FromResult(true);
        public Task<User?> GetByEmailAsync(string email) => Task.FromResult<User?>(null);
        public Task<IReadOnlyCollection<User>> SearchActiveAsync(string? search, int limit) => Task.FromResult<IReadOnlyCollection<User>>([]);
        public Task<IReadOnlyCollection<User>> SearchAllAsync(string? search, int limit) => Task.FromResult<IReadOnlyCollection<User>>([]);
        public Task<bool> SetActiveAsync(string id, bool active) => Task.FromResult(false);
        public Task<bool> DeleteAsync(string id) => Task.FromResult(false);
        public Task<User> AddAsync(User value) => Task.FromResult(value);
        public Task<bool> SetPasswordIfEmptyAsync(string email, string passwordHash) => Task.FromResult(false);
        public Task<bool> UpdatePasswordAsync(string email, string passwordHash) => Task.FromResult(false);
    }

    private sealed class Tenancy : ITenancyProvisioner
    {
        public Task<TenantAccess> ProvisionOwnerAsync(string userId, OrganizationRegistration organization, string idempotencyKey) =>
            Task.FromResult(new TenantAccess("tenant-a", "membership-a", "Owner", "Trialing", DateTime.UtcNow, DateTime.UtcNow.AddDays(30)));
        public Task CompensateOwnerAsync(string tenantId, string userId) => Task.CompletedTask;
    }

    private sealed class Tokens : IJwtTokenGenerator
    {
        public string Generate(User user) => "tenant-token";
    }
}
