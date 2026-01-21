namespace Vyracare.Auth.DTOS;

public record RegisterRequest(
    string Email,
    string? Password,
    string? FullName,
    string? Role,
    string? Department,
    string? Phone,
    string? AccessLevel,
    bool? Active
);

public record FirstAccessCheckRequest(string Email);
public record FirstAccessSetPasswordRequest(string Email, string Password);

public record LoginRequest(string Email, string Password);
