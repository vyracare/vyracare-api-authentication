using Vyracare.Auth.Common.Results;
using Vyracare.Auth.Features.Auth.Shared.Ports;

namespace Vyracare.Auth.Features.Auth.SearchEmployees;

/// <summary>
/// Pesquisa funcionarios ativos para seletores de fluxos operacionais.
/// </summary>
public sealed class SearchEmployeesHandler
{
    private readonly IUserRepository _repository;

    public SearchEmployeesHandler(IUserRepository repository)
    {
        _repository = repository;
    }

    public async Task<UseCaseResult<IReadOnlyCollection<EmployeeSearchResponse>>> HandleAsync(
        string? search,
        int limit = 20)
    {
        var users = await _repository.SearchActiveAsync(search, limit <= 0 ? 20 : limit);
        var response = users.Select(user => new EmployeeSearchResponse(
            user.Id ?? string.Empty,
            user.FullName ?? user.Email,
            user.Email,
            user.Phone,
            user.Role)).ToArray();

        return UseCaseResult<IReadOnlyCollection<EmployeeSearchResponse>>.Success(response);
    }
}
