using ClinicApp.Domain.Common;
using ClinicApp.Domain.Common.Entities;

namespace ClinicApp.Domain.SecretaryAgg;

public class Secretary : Member
{
    public Secretary(Guid id, Guid userId, string firstName, string lastName) : base(id, userId, firstName, lastName)
    {
    }
}
