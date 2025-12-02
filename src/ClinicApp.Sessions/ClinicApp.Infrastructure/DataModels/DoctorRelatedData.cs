namespace ClinicApp.Infrastructure.Persistance.DataModels;
public class TimeOffDataModel
{
    public DateTimeOffset StartDate { get; set; }
    public DateTimeOffset EndDate { get; set; }
    public string? reason { get; set; }
}
