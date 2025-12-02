namespace ClinicApp.Domain.Common.Entities;

public class Room : AggregateRoot
{
    public Room(Guid id , string name) : base(id)
    {
        Name = name;
    }
    public string Name { get; private set; } = null!;

    public void UpdateName(string name)
    {
        Name = name;
    }
}
