using ClinicApp.Application.DTOs;

namespace ClinicApp.Application.QueryServices;
public interface IDoctorQueryService
{
    Task<DoctorWithSessionsDTO?> GetDoctorWithSessions(Guid doctorid);
}
