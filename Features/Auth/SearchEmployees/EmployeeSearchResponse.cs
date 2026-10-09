namespace Vyracare.Auth.Features.Auth.SearchEmployees;

/// <summary>
/// Dados publicos e operacionais de um funcionario disponivel para selecao.
/// </summary>
public sealed record EmployeeSearchResponse(
    string Id,
    string FullName,
    string Email,
    string? Phone,
    string? Role);
