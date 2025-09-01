using ClinicApp.Domain.Common.Interfaces;

namespace ClinicApp.Domain.Common;

public class SystemClock : IClock
{
    public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;
}
