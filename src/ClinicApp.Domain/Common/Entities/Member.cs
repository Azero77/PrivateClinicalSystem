namespace ClinicApp.Domain.Common.Entities;

public class Member : AggregateRoot
{
    public Member(Guid id, Guid userId,string firstName, string lastName) : base(id)
    {
        FirstName = firstName;
        LastName = lastName;
        UserId = userId;
    }
    public Member()
    {
        
    }
    public Guid UserId { get; set; }
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
}
