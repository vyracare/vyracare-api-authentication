using Vyracare.Auth.Common.Results;
using Vyracare.Auth.Features.Auth.Shared.Ports;

namespace Vyracare.Auth.Features.Auth.ManageEmployees;

/// <summary>
/// Valida e atualiza os dados administrativos sem tocar na senha do funcionario.
/// </summary>
public sealed class UpdateEmployeeHandler(IUserRepository repository)
{
    /// <summary>
    /// Valida a solicitacao, preserva a credencial e persiste os campos administrativos permitidos.
    /// </summary>
    public async Task<UseCaseResult<EmployeeManagementResponse>> HandleAsync(
        string id,
        UpdateEmployeeRequest request,
        string? requesterId)
    {
        if (string.IsNullOrWhiteSpace(request.FullName) || string.IsNullOrWhiteSpace(request.Email) ||
            string.IsNullOrWhiteSpace(request.AccessLevel))
        {
            return UseCaseResult<EmployeeManagementResponse>.Failure(UseCaseErrorType.Validation, "Name, email and access level are required");
        }

        var user = await repository.GetByIdAsync(id);
        if (user is null)
        {
            return UseCaseResult<EmployeeManagementResponse>.Failure(UseCaseErrorType.NotFound, "Employee not found");
        }

        if (!request.Active && string.Equals(user.Id, requesterId, StringComparison.Ordinal))
        {
            return UseCaseResult<EmployeeManagementResponse>.Failure(UseCaseErrorType.Validation, "You cannot deactivate your own user");
        }

        var normalizedEmail = request.Email.Trim();
        var emailOwner = await repository.GetByEmailAsync(normalizedEmail);
        if (emailOwner is not null && emailOwner.Id != user.Id)
        {
            return UseCaseResult<EmployeeManagementResponse>.Failure(UseCaseErrorType.Conflict, "Email already in use");
        }

        user.FullName = request.FullName.Trim();
        user.Email = normalizedEmail;
        user.Phone = request.Phone?.Trim();
        user.Role = request.Role?.Trim();
        user.Department = request.Department?.Trim();
        user.AccessLevel = request.AccessLevel.Trim();
        user.Active = request.Active;

        await repository.UpdateAsync(user);
        return UseCaseResult<EmployeeManagementResponse>.Success(EmployeeManagementMapper.ToResponse(user));
    }
}
