using ClinicApp.Domain.DoctorAgg;
using ClinicApp.Shared.QueryTypes;
using HotChocolate.Authorization;

namespace ClinicApp.Shared.QueryTypes;

[Authorize(Policy = PoliciesConstants.CanViewDoctorsInfo)]
public class DoctorQueryType : MemberQueryType
{
    public string? Major { get; set; }
    public Guid RoomId { get; set; }
    public RoomQueryType? Room { get; set; }
    [UsePaging]                     
    [UseFiltering]                  
    [UseSorting] 
    public IReadOnlyCollection<SessionQueryType>? Sessions { get; set; } = new List<SessionQueryType>();
    [Authorize(PoliciesConstants.CanViewDoctorWorkingTime)]
    public WorkingTimeQueryType? WorkingTime { get; set; }
    public string TimeZoneId { get; set; } = string.Empty;
    public ICollection<TimeOffQueryType> TimesOff { get; set; } = new List<TimeOffQueryType>();
}


public class WorkingTimeQueryType : QueryType
{
    public WorkingDays WorkingDays { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }

}