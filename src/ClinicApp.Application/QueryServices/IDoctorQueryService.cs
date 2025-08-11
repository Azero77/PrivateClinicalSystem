using ClinicApp.Application.DTOs;

namespace ClinicApp.Application.QueryServices;
public interface IDoctorQueryService
{
    Task<DoctorWithSessionsDTO?> GetDoctorWithSessions(Guid doctorid);
    Task<DoctorWithSessionsDTO?> GetDoctorWithSessionsForDay(Guid doctorid, DateTime day);
}
