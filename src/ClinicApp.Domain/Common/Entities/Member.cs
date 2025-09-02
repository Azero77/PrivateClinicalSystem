namespace ClinicApp.Domain.Common.Entities;

public class Member : AggregateRoot
{
    public Member(Guid id, Guid userId, string firstName, string lastName) : base(id)
    {
        UserId = userId;
        FirstName = firstName;
        LastName = lastName;
    }
    public Guid UserId { get; private set; }
    public string FirstName { get; private set; } = null!;
    public string LastName { get; private set; } = null!;
}
