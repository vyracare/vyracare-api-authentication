using System.Security.Cryptography;
using System.Text;
using Vyracare.Auth.Features.Auth.Shared.Ports;

namespace Vyracare.Auth.Infrastructure.Security;

/// <summary>
/// Representa uma parte da arquitetura desta API.
/// </summary>
public sealed class Sha256PasswordHasher : IPasswordHasher
{
/// <summary>
/// Calcula o hash seguro de um valor sens?vel.
/// </summary>
    public string Hash(string password)
    {
        using var sha = SHA256.Create();
        var hash = sha.ComputeHash(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(hash);
    }

/// <summary>
/// Valida se o valor informado corresponde ao hash persistido.
/// </summary>
    public bool Verify(string password, string storedHash)
    {
        return Hash(password) == storedHash;
    }
}
