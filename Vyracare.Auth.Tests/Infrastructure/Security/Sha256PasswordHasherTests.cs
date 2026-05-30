using Vyracare.Auth.Infrastructure.Security;

namespace Vyracare.Auth.Tests.Infrastructure.Security;

public sealed class Sha256PasswordHasherTests
{
    [Fact]
    public void Deve_validar_hash_da_mesma_senha()
    {
        var hasher = new Sha256PasswordHasher();
        var hash = hasher.Hash("123456");

        var isValid = hasher.Verify("123456", hash);

        Assert.True(isValid);
    }
}
