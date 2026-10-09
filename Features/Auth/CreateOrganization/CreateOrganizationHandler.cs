using Vyracare.Auth.Common.Results;
using Vyracare.Auth.Features.Auth.Register;
using Vyracare.Auth.Features.Auth.Shared.Domain;
using Vyracare.Auth.Features.Auth.Shared.Ports;

namespace Vyracare.Auth.Features.Auth.CreateOrganization;

public sealed class CreateOrganizationHandler
{
    private readonly IUserRepository _users;
    private readonly ITenancyProvisioner _tenancy;
    private readonly IJwtTokenGenerator _tokens;

    public CreateOrganizationHandler(IUserRepository users, ITenancyProvisioner tenancy, IJwtTokenGenerator tokens)
    {
        _users = users;
        _tenancy = tenancy;
        _tokens = tokens;
    }

    public async Task<UseCaseResult<CreateOrganizationResponse>> HandleAsync(string userId, CreateOrganizationRequest request)
    {
        if (string.IsNullOrWhiteSpace(userId))
            return UseCaseResult<CreateOrganizationResponse>.Failure(UseCaseErrorType.Unauthorized, "Authenticated user is required");
        if (string.IsNullOrWhiteSpace(request.LegalName))
            return UseCaseResult<CreateOrganizationResponse>.Failure(UseCaseErrorType.Validation, "Organization legal name is required");

        var user = await _users.GetByIdAsync(userId);
        if (user is null || !user.Active)
            return UseCaseResult<CreateOrganizationResponse>.Failure(UseCaseErrorType.NotFound, "User not found");
        if (user.TenantAccess is not null)
            return UseCaseResult<CreateOrganizationResponse>.Failure(UseCaseErrorType.Conflict, "User already has an organization");

        TenantAccess? access = null;
        var previousAccessLevel = user.AccessLevel;
        try
        {
            access = await _tenancy.ProvisionOwnerAsync(
                userId,
                new OrganizationRegistration(request.LegalName, request.TradeName, request.Document),
                $"owner-onboarding:{userId}");
            user.AccessLevel = "Administrador";
            user.TenantAccess = access;
            if (!await _users.UpdateAsync(user) || !await _users.SetTenantAccessAsync(userId, access))
                throw new InvalidOperationException("User projection could not be updated.");

            return UseCaseResult<CreateOrganizationResponse>.Success(new CreateOrganizationResponse(
                _tokens.Generate(user), access.TenantId, access.MembershipId, access.TrialEndsAtUtc));
        }
        catch
        {
            user.AccessLevel = previousAccessLevel;
            user.TenantAccess = null;
            try { await _users.UpdateAsync(user); }
            catch { /* A reconciliacao operacional trata projecoes inconsistentes. */ }
            if (access is not null)
            {
                try { await _tenancy.CompensateOwnerAsync(access.TenantId, userId); }
                catch { /* A reconciliacao operacional trata compensacoes indisponiveis. */ }
            }
            return UseCaseResult<CreateOrganizationResponse>.Failure(UseCaseErrorType.Unavailable, "Tenant provisioning failed");
        }
    }
}
