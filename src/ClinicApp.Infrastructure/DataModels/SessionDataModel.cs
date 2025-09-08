using ClinicApp.Domain.Common.ValueObjects;
using ClinicApp.Domain.SessionAgg;

namespace ClinicApp.Infrastructure.Persistance.DataModels;

public class SessionDataModel
{
    public Guid Id { get; set; }
    public TimeRange SessionDate { get; set; } = null!;
    public SessionDescription SessionDescription { get; set; } = null!;
    public Guid RoomId { get; set; }
    public RoomDataModel Room { get; set; } = null!;
    public Guid PatientId { get; set; }
    public PatientDataModel Patient { get; set; } = null!;
    public Guid DoctorId { get; set; }
    public DoctorDataModel Doctor { get; set; } = null!;
    public SessionStatus SessionStatus { get; set; }
    public SessionHistory SessionHistory { get; set; } = new();
    public DateTimeOffset CreatedAt { get; set; }
}