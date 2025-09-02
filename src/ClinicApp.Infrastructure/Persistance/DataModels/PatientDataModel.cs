namespace ClinicApp.Infrastructure.Persistance.DataModels;

public class PatientDataModel : MemberDataModel
{
    public ICollection<SessionDataModel> Sessions = new List<SessionDataModel>();
}

public class SecretaryDataModel : MemberDataModel
{

}