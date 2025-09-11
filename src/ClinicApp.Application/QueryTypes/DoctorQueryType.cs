using ClinicApp.Domain.DoctorAgg;

namespace ClinicApp.Application.QueryTypes;

public class DoctorQueryType : MemberQueryType
{
    public string? Major { get; set; }
    public Guid RoomId { get; set; }
    public RoomQueryType? Room { get; set; }
    public IReadOnlyCollection<SessionQueryType>? Sessions { get; set; } = new List<SessionQueryType>();
    public WorkingDays WorkingDays { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
    public string TimeZoneId { get; set; } = string.Empty;
    public ICollection<TimeOffQueryType> TimesOff { get; set; } = new List<TimeOffQueryType>();
}
