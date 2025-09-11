namespace ClinicApp.Application.QueryTypes;

public class TimeOffQueryType
{
    public DateTimeOffset StartDate { get; set; }
    public DateTimeOffset EndDate { get; set; }
    public string? reason { get; set; }
}
