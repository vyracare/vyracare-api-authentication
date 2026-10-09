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

    public async Task<TenantAccess> ProvisionMemberAsync(string tenantId, string userId, string role)
    {
        using var message = new HttpRequestMessage(HttpMethod.Post,
            $"api/tenancy/internal/tenants/{Uri.EscapeDataString(tenantId)}/memberships")
        {
            Content = JsonContent.Create(new { userId, role })
        };
        AddInternalApiKey(message);
        using var response = await _httpClient.SendAsync(message);
        response.EnsureSuccessStatusCode();
        var membership = await response.Content.ReadFromJsonAsync<MembershipContract>()
                         ?? throw new InvalidOperationException("Tenancy returned an empty membership response.");
        return Map(membership);
    }

    public async Task CompensateMemberAsync(string tenantId, string userId)
    {
        using var message = new HttpRequestMessage(HttpMethod.Delete,
            $"api/tenancy/internal/tenants/{Uri.EscapeDataString(tenantId)}/memberships/{Uri.EscapeDataString(userId)}");
        AddInternalApiKey(message);
        using var response = await _httpClient.SendAsync(message);
        response.EnsureSuccessStatusCode();
    }

    public async Task<IReadOnlyCollection<TenantAccess>> GetMembershipsAsync(string userId)
    {
        using var message = new HttpRequestMessage(HttpMethod.Get,
            $"api/tenancy/internal/users/{Uri.EscapeDataString(userId)}/memberships");
        AddInternalApiKey(message);
        using var response = await _httpClient.SendAsync(message);
        response.EnsureSuccessStatusCode();
        var memberships = await response.Content.ReadFromJsonAsync<IReadOnlyCollection<MembershipContract>>() ?? [];
        return memberships.Select(Map).ToArray();
    }

    private void AddInternalApiKey(HttpRequestMessage message) =>
        message.Headers.Add("X-Internal-Api-Key", _options.InternalApiKey);

    private static TenantAccess Map(MembershipContract item) => new(
        item.TenantId, item.MembershipId, item.Role, item.TenantStatus,
        item.TrialStartsAtUtc, item.TrialEndsAtUtc);

    private sealed record MembershipContract(
        string TenantId,
        string MembershipId,
        string Role,
        string TenantStatus,
        DateTime TrialStartsAtUtc,
        DateTime TrialEndsAtUtc);
}
