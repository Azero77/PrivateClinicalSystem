using ClinicApp.Domain.DoctorAgg;

namespace ClinicApp.Infrastructure.Persistance.DataModels;
public class DoctorDataModel
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public WorkingTime WorkingTime { get; set; } = null!;
    public string? Major { get; set; }
    public Guid RoomId { get; set; }
    public RoomDataModel Room { get; set; } = null!;

    public IReadOnlyCollection<SessionDataModel> Sessions = new List<SessionDataModel>();
}
