using ClinicApp.Domain.Common.ValueObjects;
using ClinicApp.Domain.SessionAgg;

namespace ClinicApp.Application.QueryTypes;
public class SessionQueryType : QueryType
{
    public TimeRange SessionDate { get; set; } = null!;
    public SessionDescription SessionDescription { get; set; } = null!;
    public Guid RoomId { get; set; }
    public RoomQueryType Room { get; set; } = null!;
    public Guid PatientId { get; set; }
    public PatientQueryType Patient { get; set; } = null!;
    public Guid DoctorId { get; set; }
    public DoctorQueryType Doctor { get; set; } = null!;
    public SessionStatus SessionStatus { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}
