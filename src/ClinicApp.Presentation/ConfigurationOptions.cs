namespace ClinicApp.Presentation;

public class QueryOptions
{
    public int MaxPageSize { get; set; }
    public int DefaultPageSize { get; set; }
    public bool IncludeTotalCount { get; set; } 
}