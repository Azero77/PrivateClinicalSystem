using ClinicApp.Domain.SessionAgg;

public record AddSessionRequest(
    DateTimeOffset StartTime,
    DateTimeOffset EndTime,
    SessionDescription SessionDescription,
    Guid roomId,
    Guid patientId,
    Guid doctorId
    );
