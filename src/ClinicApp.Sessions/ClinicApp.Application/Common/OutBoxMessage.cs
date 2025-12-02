namespace ClinicApp.Application.Common;
public record OutBoxMessage(Guid Id,
    string Name,
    string Content, //the event serialized to json
    DateTime CreatedAt,
    DateTime? ManagedAt = null,
    string? error = null);
