using Vyracare.Auth.Common.Results;
using Vyracare.Auth.Features.Auth.Shared.Ports;

namespace Vyracare.Auth.Features.Auth.ManageEmployees;

/// <summary>
/// Lista funcionarios ativos e inativos para a gestao administrativa.
/// </summary>
public sealed class ListManagedEmployeesHandler(IUserRepository repository)
{
    /// <summary>
    /// Pesquisa funcionarios ativos e inativos e devolve somente a projecao administrativa segura.
    /// </summary>
    public async Task<UseCaseResult<IReadOnlyCollection<EmployeeManagementResponse>>> HandleAsync(string? search, int limit)
    {
        var users = await repository.SearchAllAsync(search, limit <= 0 ? 100 : limit);
        return UseCaseResult<IReadOnlyCollection<EmployeeManagementResponse>>.Success(
            users.Select(EmployeeManagementMapper.ToResponse).ToArray());
    }
}
