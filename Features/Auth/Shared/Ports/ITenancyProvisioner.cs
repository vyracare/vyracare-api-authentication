using Vyracare.Auth.Features.Auth.Register;
using Vyracare.Auth.Features.Auth.Shared.Domain;

namespace Vyracare.Auth.Features.Auth.Shared.Ports;

public interface ITenancyProvisioner
{
    Task<TenantAccess> ProvisionOwnerAsync(string userId, OrganizationRegistration organization, string idempotencyKey);
    Task CompensateOwnerAsync(string tenantId, string userId);
    Task<TenantAccess> ProvisionMemberAsync(string tenantId, string userId, string role) =>
        throw new NotSupportedException();
    Task CompensateMemberAsync(string tenantId, string userId) => Task.CompletedTask;
    Task<IReadOnlyCollection<TenantAccess>> GetMembershipsAsync(string userId) =>
        Task.FromResult<IReadOnlyCollection<TenantAccess>>([]);
}

