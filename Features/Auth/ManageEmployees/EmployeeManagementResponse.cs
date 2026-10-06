namespace Vyracare.Auth.Features.Auth.ManageEmployees;

/// <summary>
/// Projecao administrativa do funcionario sem senha, hash ou outros dados de credencial.
/// </summary>
public sealed record EmployeeManagementResponse(
    string Id,
    string FullName,
    string Email,
    string? Phone,
    string? Role,
    string? Department,
    string? AccessLevel,
    bool Active);
