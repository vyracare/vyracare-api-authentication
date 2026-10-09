using System.Net.Http.Json;
using Microsoft.Extensions.Options;
using Vyracare.Auth.Common.Configuration;
using Vyracare.Auth.Features.Auth.Register;
using Vyracare.Auth.Features.Auth.Shared.Domain;
using Vyracare.Auth.Features.Auth.Shared.Ports;

namespace Vyracare.Auth.Infrastructure.Tenancy;

public sealed class HttpTenancyProvisioner : ITenancyProvisioner
{
    private readonly HttpClient _httpClient;
    private readonly TenancyApiOptions _options;

    public HttpTenancyProvisioner(HttpClient httpClient, IOptions<TenancyApiOptions> options)
    {
        _httpClient = httpClient;
        _options = options.Value;
    }

    public async Task<TenantAccess> ProvisionOwnerAsync(string userId, OrganizationRegistration organization, string idempotencyKey)
    {
        using var message = new HttpRequestMessage(HttpMethod.Post, "api/tenancy/internal/tenants")
        {
            Content = JsonContent.Create(new
            {
                userId,
                organization.LegalName,
                organization.TradeName,
                organization.Document,
                idempotencyKey
            })
        };
        message.Headers.Add("X-Internal-Api-Key", _options.InternalApiKey);
        using var response = await _httpClient.SendAsync(message);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<TenantAccess>()
               ?? throw new InvalidOperationException("Tenancy returned an empty provisioning response.");
    }

    public async Task CompensateOwnerAsync(string tenantId, string userId)
    {
        using var message = new HttpRequestMessage(HttpMethod.Delete,
            $"api/tenancy/internal/tenants/{Uri.EscapeDataString(tenantId)}?userId={Uri.EscapeDataString(userId)}");
        message.Headers.Add("X-Internal-Api-Key", _options.InternalApiKey);
        using var response = await _httpClient.SendAsync(message);
        response.EnsureSuccessStatusCode();
    }
}
