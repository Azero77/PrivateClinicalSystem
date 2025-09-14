using ClinicApp.Shared.QueryTypes;

namespace ClinicApp.Shared.QueryTypes;

public abstract class MemberQueryType : QueryType
{
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public Guid UserId { get; set; }
}