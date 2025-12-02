namespace ClinicApp.Presentation.Requests;

public record UpdateSessionTimeRequest(Guid SessionId,DateTimeOffset StartTime,DateTimeOffset EndTime);