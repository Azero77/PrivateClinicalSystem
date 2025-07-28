using ClinicApp.Domain.Common.Interfaces;

namespace ClinicApp.Domain.Common;

public class SystemClock : IClock
{
    public DateTime UtcNow => DateTime.UtcNow;
}
