using Vyracare.Auth.Common.Results;
using Vyracare.Auth.Features.Auth.Shared.Ports;

namespace Vyracare.Auth.Features.Auth.ManageEmployees;

/// <summary>
/// Exclui definitivamente um funcionário por decisão administrativa confirmada.
/// </summary>
public sealed class DeleteEmployeeHandler(IUserRepository repository)
{
    /// <summary>
    /// Impede a autoexclusão, valida a existência do funcionário e remove seu cadastro.
    /// </summary>
    public async Task<UseCaseResult<bool>> HandleAsync(string id, string? requesterId)
    {
        if (string.Equals(id, requesterId, StringComparison.Ordinal))
        {
            return UseCaseResult<bool>.Failure(UseCaseErrorType.Validation, "You cannot delete your own user");
        }

        var user = await repository.GetByIdAsync(id);
        if (user is null)
        {
            return UseCaseResult<bool>.Failure(UseCaseErrorType.NotFound, "Employee not found");
        }

        var deleted = await repository.DeleteAsync(id);
        return deleted
            ? UseCaseResult<bool>.Success(true)
            : UseCaseResult<bool>.Failure(UseCaseErrorType.NotFound, "Employee not found");
    }
}
