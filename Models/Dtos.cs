namespace Vyracare.Auth.Models;

public record RegisterRequest(string Email, string Password, string? FullName);
public record LoginRequest(string Email, string Password);
