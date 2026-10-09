using Vyracare.Auth.Common.Results;
using Vyracare.Auth.Common.Time;
using Vyracare.Auth.Features.Auth.Register;
using Vyracare.Auth.Features.Auth.Shared.Domain;
using Vyracare.Auth.Features.Auth.Shared.Ports;

namespace Vyracare.Auth.Features.Auth.CreateEmployee;

public sealed class CreateEmployeeHandler
{
    private readonly IUserRepository _users;
    private readonly IPasswordHasher _passwords;
    private readonly IClock _clock;
    private readonly ITenancyProvisioner _tenancy;

    public CreateEmployeeHandler(
        IUserRepository users,
        IPasswordHasher passwords,
        IClock clock,
        ITenancyProvisioner tenancy) =>
        (_users, _passwords, _clock, _tenancy) = (users, passwords, clock, tenancy);

    public async Task<UseCaseResult<RegisterResponse>> HandleAsync(string tenantId, RegisterRequest request)
    {
        if (string.IsNullOrWhiteSpace(tenantId))
            return UseCaseResult<RegisterResponse>.Failure(UseCaseErrorType.Unauthorized, "Tenant context is required");
        if (string.IsNullOrWhiteSpace(request.Email))
            return UseCaseResult<RegisterResponse>.Failure(UseCaseErrorType.Validation, "Email is required");
        if (request.Organization is not null)
            return UseCaseResult<RegisterResponse>.Failure(UseCaseErrorType.Validation, "Employee cannot create an organization");

        var email = request.Email.Trim();
        if (await _users.GetByEmailAsync(email) is not null)
            return UseCaseResult<RegisterResponse>.Failure(UseCaseErrorType.Conflict, "User already exists");

        var employee = await _users.AddAsync(new User
        {
            Email = email,
            FullName = request.FullName?.Trim(),
            Role = request.Role?.Trim(),
            Department = request.Department?.Trim(),
            Phone = request.Phone?.Trim(),
            AccessLevel = request.AccessLevel?.Trim(),
            Active = request.Active ?? true,
            PasswordHash = string.IsNullOrWhiteSpace(request.Password) ? string.Empty : _passwords.Hash(request.Password),
            CreatedAt = _clock.UtcNow
        });

        if (string.IsNullOrWhiteSpace(employee.Id))
            return UseCaseResult<RegisterResponse>.Failure(UseCaseErrorType.Unavailable, "Employee could not be persisted");

        TenantAccess? access = null;
        try
        {
            var tenantRole = string.Equals(employee.AccessLevel, "Administrador", StringComparison.OrdinalIgnoreCase)
                ? "Administrator"
                : "Member";
            access = await _tenancy.ProvisionMemberAsync(tenantId, employee.Id, tenantRole);
            employee.TenantAccess = access;
            if (!await _users.SetTenantAccessAsync(employee.Id, access))
                throw new InvalidOperationException("Tenant projection could not be persisted.");
            return UseCaseResult<RegisterResponse>.Success(new RegisterResponse(employee.Id, "Employee created"));
        }
        catch
        {
            if (access is not null)
            {
                try { await _tenancy.CompensateMemberAsync(tenantId, employee.Id); }
                catch { /* A reconciliacao operacional remove vinculos orfaos. */ }
            }
            await _users.DeleteAsync(employee.Id);
            return UseCaseResult<RegisterResponse>.Failure(UseCaseErrorType.Unavailable, "Employee tenant provisioning failed");
        }
    }
}
