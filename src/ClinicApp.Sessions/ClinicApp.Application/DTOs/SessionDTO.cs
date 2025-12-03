using ClinicApp.Domain.SessionAgg;
using System.Text.Json;

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
    public JsonElement? SessionDescription { get; set; }

}
