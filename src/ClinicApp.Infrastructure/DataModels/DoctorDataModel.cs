using ClinicApp.Domain.DoctorAgg;

namespace ClinicApp.Infrastructure.Persistance.DataModels;
public class DoctorDataModel : MemberDataModel
{
    public WorkingTime WorkingTime { get; set; } = null!;
    public string? Major { get; set; }
    public Guid RoomId { get; set; }
    public RoomDataModel Room { get; set; } = null!;

    public IReadOnlyCollection<SessionDataModel> Sessions = new List<SessionDataModel>();
}

public abstract class DataModel
{
    public Guid Id { get; set; }
}

public abstract class MemberDataModel : DataModel
{
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public Guid UserId { get; set; }
}