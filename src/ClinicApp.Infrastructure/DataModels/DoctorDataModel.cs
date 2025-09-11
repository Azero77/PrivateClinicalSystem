using ClinicApp.Domain.DoctorAgg;

namespace ClinicApp.Infrastructure.Persistance.DataModels;
public class DoctorDataModel : MemberDataModel
{
    public string? Major { get; set; }
    public Guid RoomId { get; set; }
    public RoomDataModel? Room { get; set; }
    public ICollection<SessionDataModel>? Sessions { get; set; } = new List<SessionDataModel>();
    public WorkingDays WorkingDays { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
    public string TimeZoneId { get; set; } = string.Empty;
    public ICollection<TimeOffDataModel> TimesOff { get; set; } = new List<TimeOffDataModel>();
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