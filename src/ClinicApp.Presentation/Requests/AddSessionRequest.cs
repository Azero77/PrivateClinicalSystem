using ClinicApp.Domain.SessionAgg;

public record AddSessionRequest(
    DateTime StartTime,
    DateTime EndTime,
    SessionDescription SessionDescription,
    Guid roomId,
    Guid patientId,
    Guid doctorId
    );
