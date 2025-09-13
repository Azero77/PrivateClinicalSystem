namespace ClinicApp.Presentation.Requests;

public record CreateSecretaryRequest(string FirstName, string LastName, Guid UserId);
