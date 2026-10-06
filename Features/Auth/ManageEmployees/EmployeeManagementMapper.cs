using Vyracare.Auth.Features.Auth.Shared.Domain;

namespace Vyracare.Auth.Features.Auth.ManageEmployees;

internal static class EmployeeManagementMapper
{
    /// <summary>
    /// Converte o usuario interno na projecao minima permitida para administracao.
    /// </summary>
    internal static EmployeeManagementResponse ToResponse(User user) => new(
        user.Id ?? string.Empty,
        user.FullName ?? user.Email,
        user.Email,
        user.Phone,
        user.Role,
        user.Department,
        user.AccessLevel,
        user.Active);
}
