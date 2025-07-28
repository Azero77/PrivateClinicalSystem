namespace ClinicApp.Domain.Common.Interfaces;

public interface IClock
{
    public DateTime UtcNow => DateTime.UtcNow;
}
