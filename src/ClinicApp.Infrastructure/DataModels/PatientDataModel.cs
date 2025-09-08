namespace ClinicApp.Infrastructure.Persistance.DataModels;

public class PatientDataModel
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
}
