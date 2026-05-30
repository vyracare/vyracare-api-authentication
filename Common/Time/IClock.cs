namespace Vyracare.Auth.Common.Time;

/// <summary>
/// Representa o componente respons?vel por fornecer a data e hora atual para a aplica??o.
/// </summary>
public interface IClock
{
    DateTime UtcNow { get; }
}
