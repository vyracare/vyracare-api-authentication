namespace Vyracare.Auth.Common.Time;

/// <summary>
/// Define uma abstração de relógio para a aplicação.
/// Essa interface evita dependência direta de <see cref="DateTime.UtcNow"/> nos handlers
/// e facilita testes em cenários em que a noção de tempo precisa ser controlada.
/// </summary>
public interface IClock
{
    /// <summary>
    /// Obtém a data e hora atual em UTC segundo a implementação configurada.
    /// </summary>
    DateTime UtcNow { get; }
}
