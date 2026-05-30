namespace Vyracare.Auth.Common.Configuration;

public sealed class JwtOptions
{
    public const string SectionName = "Jwt";

    public string Key { get; set; } = string.Empty;
    public string Issuer { get; set; } = "vyracare-auth";
    public string Audience { get; set; } = "vyracare-client";
    public int ExpiryMinutes { get; set; } = 60;
}
