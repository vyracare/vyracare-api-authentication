using Vyracare.Auth.Common.Time;

namespace Vyracare.Auth.Infrastructure.Time;

public sealed class SystemClock : IClock
{
    public DateTime UtcNow => DateTime.UtcNow;
}
