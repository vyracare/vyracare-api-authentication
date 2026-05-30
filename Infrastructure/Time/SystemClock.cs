using Vyracare.Auth.Common.Time;

namespace Vyracare.Auth.Infrastructure.Time;

/// <summary>
/// Representa o componente respons?vel por fornecer a data e hora atual para a aplica??o.
/// </summary>
public sealed class SystemClock : IClock
{
    public DateTime UtcNow => DateTime.UtcNow;
}
