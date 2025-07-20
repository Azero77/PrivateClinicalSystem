using ClinicApp.Domain.Session;

namespace ClinicApp.Domain.Repositories;

public interface ISessionRepository
{
    Task<IEnumerable<Session.Session>> GetFutureSessionsDoctor(Doctor.Doctor doctor);
    Task<IEnumerable<Session.Session>> GetAllSessionsForDoctor(Doctor.Doctor doctor);
}
