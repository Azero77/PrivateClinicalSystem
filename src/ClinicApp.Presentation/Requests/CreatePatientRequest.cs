namespace ClinicApp.Presentation.Requests;

public record CreatePatientRequest(string FirstName, string LastName, Guid UserId);
