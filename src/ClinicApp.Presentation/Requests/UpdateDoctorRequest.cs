using ClinicApp.Domain.DoctorAgg;
namespace ClinicApp.Presentation.Requests;
public record UpdateDoctorRequest(string? FirstName, string? LastName, Guid? RoomId, WorkingDays? WorkingDays, TimeOnly? WorkingHoursStartTime, TimeOnly? WorkingHoursEndTime, string? Major);
