using System.Linq.Expressions;
using ClinicApp.Infrastructure.Persistance.DataModels;

namespace ClinicApp.Application.QueryTypes;

public class PatientQueryType : MemberQueryType
{
    public ICollection<SessionQueryType>? Sessions { get; set; } = null;
}
