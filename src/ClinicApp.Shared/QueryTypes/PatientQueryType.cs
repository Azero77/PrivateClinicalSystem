using ClinicApp.Shared.QueryTypes;

namespace ClinicApp.Shared.QueryTypes;

public class PatientQueryType : MemberQueryType
{
    public ICollection<SessionQueryType>? Sessions { get; set; } = null;
}
