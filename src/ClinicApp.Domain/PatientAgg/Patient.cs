using ClinicApp.Domain.Common;
using ClinicApp.Domain.Common.Entities;

namespace ClinicApp.Domain.PatientAgg;

public class Patient : Member
{
    public Patient(Guid id, Guid userId, string firstName, string lastName) : base(id,userId,firstName,lastName)
    {
        FirstName = firstName;
        LastName = lastName;
        UserId = userId;
    }
}
