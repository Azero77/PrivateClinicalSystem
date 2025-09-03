namespace ClinicApp.Domain.Common.Entities;

public class Room : Entity
{
    public Room(Guid id , string name) : base(id)
    {
        Name = name;
    }
    public string Name { get; private set; } = null!;
}
