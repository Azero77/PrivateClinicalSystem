using ClinicApp.Domain.Session;

namespace ClinicApp.Domain.Repositories;

public interface ISessionRepository
{
    Task<IEnumerable<Session.Session>> GetNewSessionsForDoctor(Doctor.Doctor doctor);
}
