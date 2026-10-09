using Vyracare.Auth.Features.Auth.Register;
using Vyracare.Auth.Features.Auth.Shared.Domain;

namespace Vyracare.Auth.Features.Auth.Shared.Ports;

public interface ITenancyProvisioner
{
    Task<TenantAccess> ProvisionOwnerAsync(string userId, OrganizationRegistration organization, string idempotencyKey);
    Task CompensateOwnerAsync(string tenantId, string userId);
}

