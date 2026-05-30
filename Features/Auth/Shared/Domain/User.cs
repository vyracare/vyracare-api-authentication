namespace Vyracare.Auth.Features.Auth.Shared.Domain;

public sealed class User
{
    public string? Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string? FullName { get; set; }
    public string? Role { get; set; }
    public string? Department { get; set; }
    public string? Phone { get; set; }
    public string? AccessLevel { get; set; }
    public bool Active { get; set; } = true;
    public string PasswordHash { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
