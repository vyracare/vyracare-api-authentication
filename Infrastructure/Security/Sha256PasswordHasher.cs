using System.Security.Cryptography;
using System.Text;
using Vyracare.Auth.Features.Auth.Shared.Ports;

namespace Vyracare.Auth.Infrastructure.Security;

/// <summary>
/// Implementa a geração e a verificação de hashes usados pela aplicação.
/// </summary>
public sealed class Sha256PasswordHasher : IPasswordHasher
{
/// <summary>
/// Calcula o hash seguro do valor informado.
/// </summary>
    public string Hash(string password)
    {
        using var sha = SHA256.Create();
        var hash = sha.ComputeHash(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(hash);
    }

/// <summary>
/// Verifica se o valor informado corresponde ao hash armazenado.
/// </summary>
    public bool Verify(string password, string storedHash)
    {
        return Hash(password) == storedHash;
    }
}
