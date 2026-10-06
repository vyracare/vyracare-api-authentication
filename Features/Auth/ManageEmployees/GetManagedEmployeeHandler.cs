using Vyracare.Auth.Common.Results;
using Vyracare.Auth.Features.Auth.Shared.Ports;

namespace Vyracare.Auth.Features.Auth.ManageEmployees;

/// <summary>
/// Recupera os dados editaveis de um funcionario sem expor sua credencial.
/// </summary>
public sealed class GetManagedEmployeeHandler(IUserRepository repository)
{
    public async Task<UseCaseResult<EmployeeManagementResponse>> HandleAsync(string id)
    {
        var user = await repository.GetByIdAsync(id);
        return user is null
            ? UseCaseResult<EmployeeManagementResponse>.Failure(UseCaseErrorType.NotFound, "Employee not found")
            : UseCaseResult<EmployeeManagementResponse>.Success(EmployeeManagementMapper.ToResponse(user));
    }
}
