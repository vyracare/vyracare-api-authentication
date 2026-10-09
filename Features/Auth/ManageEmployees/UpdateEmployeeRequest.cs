namespace Vyracare.Auth.Features.Auth.ManageEmployees;

/// <summary>
/// Dados administrativos permitidos na edicao de um funcionario.
/// </summary>
public sealed record UpdateEmployeeRequest(
    string FullName,
    string Email,
    string? Phone,
    string? Role,
    string? Department,
    string AccessLevel,
    bool Active);
