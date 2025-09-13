namespace ClinicApp.Infrastructure.Persistance.DataModels;

public class PatientDataModel : MemberDataModel
{
    public ICollection<SessionDataModel>? Sessions { get; set; } = null;
}

public class SecretaryDataModel : MemberDataModel
{
}