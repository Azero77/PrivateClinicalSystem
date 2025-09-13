using ClinicApp.Domain.DoctorAgg;
namespace ClinicApp.Presentation.Requests;
public record CreateDoctorRequest(Guid UserId, Guid RoomId, string FirstName, string LastName, WorkingDays WorkingDays, TimeOnly WorkingHoursStartTime, TimeOnly WorkingHoursEndTime, string? Major);
