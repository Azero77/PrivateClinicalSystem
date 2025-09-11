using ClinicApp.Domain.SessionAgg;

public record AddSessionRequest(
    DateTimeOffset StartTime,
    DateTimeOffset EndTime,
    string SessionDescriptionContent,
    Guid RoomId,
    Guid PatientId,
    Guid DoctorId
    );
