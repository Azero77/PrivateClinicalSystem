using ClinicApp.Domain.SessionAgg;
using ClinicApp.Shared.QueryTypes;
using HotChocolate.Authorization;

namespace ClinicApp.Shared.QueryTypes;
public class SessionQueryType : QueryType
{
    public DateTimeOffset StartTime { get; set; }
    public DateTimeOffset EndTime { get; set; }
    [Authorize(Policy = PoliciesConstants.CanViewSessionDetails,Apply = ApplyPolicy.AfterResolver)]
    public string? Content { get; set; }
    public Guid RoomId { get; set; }
    public RoomQueryType? Room { get; set; }
    public Guid PatientId { get; set; }
    public PatientQueryType? Patient { get; set; }
    public Guid DoctorId { get; set; }
    public DoctorQueryType? Doctor { get; set; }
    public SessionStatus SessionStatus { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}
