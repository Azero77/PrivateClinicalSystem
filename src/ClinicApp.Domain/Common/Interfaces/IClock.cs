namespace ClinicApp.Domain.Common.Interfaces;

public interface IClock
{
    public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;
}
