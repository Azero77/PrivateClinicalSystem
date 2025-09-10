using ClinicApp.Domain.DoctorAgg;

namespace ClinicApp.Application.QueryTypes;

public class DoctorQueryType : MemberQueryType
{
    public WorkingTime WorkingTime { get; set; } = null!;
    public string? Major { get; set; }
    public Guid RoomId { get; set; }
    public RoomQueryType Room { get; set; } = null!;

    public IReadOnlyCollection<SessionQueryType> Sessions { get; set; } = new List<SessionQueryType>();
}
