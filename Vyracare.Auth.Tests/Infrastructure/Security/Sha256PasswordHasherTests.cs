using Vyracare.Auth.Infrastructure.Security;

namespace Vyracare.Auth.Tests.Infrastructure.Security;

/// <summary>
/// Representa o componente Sha256PasswordHasherTests da aplicação.
/// </summary>
public sealed class Sha256PasswordHasherTests
{
    [Fact]
/// <summary>
/// Executa a responsabilidade do método D ev e_v al id ar_h as h_d a_m es ma_s en ha.
/// </summary>
    public void Deve_validar_hash_da_mesma_senha()
    {
        var hasher = new Sha256PasswordHasher();
        var hash = hasher.Hash("123456");

        var isValid = hasher.Verify("123456", hash);

        Assert.True(isValid);
    }
}
