namespace ClinicApp.Shared.QueryTypes;

public class TimeOffQueryType : QueryType
{
    public DateTimeOffset StartDate { get; set; }
    public DateTimeOffset EndDate { get; set; }
    public string? reason { get; set; }
}
