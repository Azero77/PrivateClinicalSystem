using ClinicApp.Domain.SessionAgg;

namespace ClinicApp.Application.DTOs;
public class SessionDTO
{
    public Guid Id { get; set; }
    public DateTimeOffset StartTime { get; set; }
    public DateTimeOffset EndTime { get; set; }
    public Guid RoomId { get; set; }
    public Guid DoctorId { get; set; }
    public Guid PatientId { get; set; }
    public SessionStatus SessionStatus { get; set; }
    public string SessionDescription { get; set; } = string.Empty;

}
