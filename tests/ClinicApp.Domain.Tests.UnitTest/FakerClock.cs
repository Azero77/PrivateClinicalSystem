using ClinicApp.Domain.Common.Interfaces;

namespace ClinicApp.Domain.Tests.UnitTest;

internal class FakerClock : IClock
{
    public DateTime UtcNow { get; set; }

}
