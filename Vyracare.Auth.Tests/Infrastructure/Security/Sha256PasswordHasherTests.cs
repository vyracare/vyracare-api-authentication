using Vyracare.Auth.Infrastructure.Security;

namespace Vyracare.Auth.Tests.Infrastructure.Security;

/// <summary>
/// Agrupa os cen?rios de teste unit?rio relacionados a este componente.
/// </summary>
public sealed class Sha256PasswordHasherTests
{
    [Fact]
/// <summary>
/// Executa a responsabilidade associada a d ev e v al id ar h as h d a m es ma s en ha.
/// </summary>
    public void Deve_validar_hash_da_mesma_senha()
    {
        var hasher = new Sha256PasswordHasher();
        var hash = hasher.Hash("123456");

        var isValid = hasher.Verify("123456", hash);

        Assert.True(isValid);
    }
}
