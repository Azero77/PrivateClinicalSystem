using ClinicApp.Domain.SessionAgg;
using System.Text.Json;

public record AddSessionRequest(
    DateTimeOffset StartTime,
    DateTimeOffset EndTime,
    JsonElement SessionDescriptionContent,
    Guid RoomId,
    Guid PatientId,
    Guid DoctorId
    );
