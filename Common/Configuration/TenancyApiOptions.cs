namespace Vyracare.Auth.Common.Configuration;

public sealed class TenancyApiOptions
{
    public const string SectionName = "TenancyApi";
    public string BaseUrl { get; set; } = "http://localhost:5006";
    public string InternalApiKey { get; set; } = string.Empty;
}

