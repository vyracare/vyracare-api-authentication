using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.Extensions.Options;
using Vyracare.Auth.Common.Configuration;
using Vyracare.Auth.Features.Auth.Shared.Domain;
using Vyracare.Auth.Infrastructure.Security;

namespace Vyracare.Auth.Tests.Infrastructure.Security;

public sealed class JwtTokenGeneratorTests
{
    [Fact]
    public void Deve_incluir_nivel_de_acesso_como_role_e_cargo_no_token()
    {
        var options = Options.Create(new JwtOptions
        {
            Key = "uma-chave-de-testes-com-tamanho-suficiente-123456",
            Issuer = "vyracare-tests",
            Audience = "vyracare-tests",
            ExpiryMinutes = 30
        });
        var generator = new JwtTokenGenerator(options);

        var token = generator.Generate(new User
        {
            Id = "user-1",
            Email = "admin@vyracare.com",
            FullName = "Admin Vyracare",
            AccessLevel = "Administrador",
            Role = "Clinico",
            TenantAccess = new TenantAccess(
                "tenant-1", "membership-1", "Owner", "Trialing",
                DateTime.UtcNow, DateTime.UtcNow.AddDays(30))
        });

        var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);

        Assert.Contains(jwt.Claims, claim => claim.Type == ClaimTypes.Role && claim.Value == "Administrador");
        Assert.Contains(jwt.Claims, claim => claim.Type == "access_level" && claim.Value == "Administrador");
        Assert.Contains(jwt.Claims, claim => claim.Type == "job_role" && claim.Value == "Clinico");
        Assert.Contains(jwt.Claims, claim => claim.Type == "tenant_id" && claim.Value == "tenant-1");
        Assert.Contains(jwt.Claims, claim => claim.Type == "membership_id" && claim.Value == "membership-1");
        Assert.Contains(jwt.Claims, claim => claim.Type == "tenant_role" && claim.Value == "Owner");
        Assert.Contains(jwt.Claims, claim => claim.Type == "plan" && claim.Value == "trial");
    }
}
