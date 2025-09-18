using ClinicApp.Domain.SessionAgg;

namespace ClinicApp.Infrastructure.Persistance.DataModels;

public class SessionDataModel : DataModel
{
    public DateTimeOffset StartTime { get; set; }
    public DateTimeOffset EndTime { get; set; }
    public string? Content { get; set; }
    public Guid RoomId { get; set; }
    public RoomDataModel? Room { get; set; }
    public Guid PatientId { get; set; }
    public PatientDataModel? Patient { get; set; }
    public Guid DoctorId { get; set; }
    public DoctorDataModel? Doctor { get; set; }
    public SessionStatus SessionStatus { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}