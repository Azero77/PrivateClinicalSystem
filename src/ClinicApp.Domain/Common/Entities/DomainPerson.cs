namespace ClinicApp.Domain.Common.Entities;

public class DomainPerson : AggregateRoot
{
    public string FirstName { get; private set; } = null!;
    public string LastName { get; private set; } = null!;
    public string Address { get; private set; } = null!;
}
