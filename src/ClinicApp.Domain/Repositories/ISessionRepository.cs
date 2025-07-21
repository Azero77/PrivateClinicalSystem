using ClinicApp.Domain.Session;
using ErrorOr;

namespace ClinicApp.Domain.Repositories;

public interface ISessionRepository
{
    Task<IEnumerable<Session.Session>> GetFutureSessionsDoctor(Doctor.Doctor doctor);
    Task<IEnumerable<Session.Session>> GetAllSessionsForDoctor(Doctor.Doctor doctor);
    Task RemoveSessionFromDoctor(Session.Session session);
}
