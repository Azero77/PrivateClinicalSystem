namespace ClinicApp.Domain.Common.Entities;

public class Member : AggregateRoot
{
    public Member(Guid id) : base(id)
    {
        
    }
    public Member()
    {
        
    }
    public Guid UserId { get; set; }
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
}
