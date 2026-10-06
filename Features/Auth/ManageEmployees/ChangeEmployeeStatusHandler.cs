using Vyracare.Auth.Common.Results;
using Vyracare.Auth.Features.Auth.Shared.Ports;

namespace Vyracare.Auth.Features.Auth.ManageEmployees;

/// <summary>
/// Ativa ou inativa um funcionario, impedindo a autoinativacao do administrador atual.
/// </summary>
public sealed class ChangeEmployeeStatusHandler(IUserRepository repository)
{
    public async Task<UseCaseResult<EmployeeManagementResponse>> HandleAsync(
        string id,
        ChangeEmployeeStatusRequest request,
        string? requesterId)
    {
        if (!request.Active && string.Equals(id, requesterId, StringComparison.Ordinal))
        {
            return UseCaseResult<EmployeeManagementResponse>.Failure(UseCaseErrorType.Validation, "You cannot deactivate your own user");
        }

        var user = await repository.GetByIdAsync(id);
        if (user is null)
        {
            return UseCaseResult<EmployeeManagementResponse>.Failure(UseCaseErrorType.NotFound, "Employee not found");
        }

        await repository.SetActiveAsync(id, request.Active);
        user.Active = request.Active;
        return UseCaseResult<EmployeeManagementResponse>.Success(EmployeeManagementMapper.ToResponse(user));
    }
}
