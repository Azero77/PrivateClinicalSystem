using ClinicApp.Domain.Session;
using ErrorOr;

namespace ClinicApp.Domain.Repositories;

public interface ISessionRepository
{
    Task<IEnumerable<Session.Session>> GetFutureSessionsDoctor(Doctor.Doctor doctor);
    Task<IEnumerable<Session.Session>> GetAllSessionsForDoctor(Doctor.Doctor doctor);
    Task<ErrorOr<Session.Session>> GetSessionById(Guid sessionId);
    Task<IEnumerable<Session.Session>> GetSessionsForToday();
    Task<IEnumerable<Session.Session>> GetSessionsForDoctorToday(Guid doctorid);
    Task<IEnumerable<Session.Session>> GetSessionsForDay(DateOnly date);
    Task<ErrorOr<Created>> AddSession(Session.Session session);
    Task<ErrorOr<Success>> SetSession(Session.Session session);
    Task<ErrorOr<Updated>> UpdateSession(Session.Session session);
    Task<ErrorOr<Success>> RejectSession(Session.Session session);
    Task<ErrorOr<Deleted>> DeleteSession(Guid sessionId);
    Task<IEnumerable<SessionState>> GetSessionHistory(Guid sessionId);
    Task RemoveSessionFromDoctor(Session.Session session);
}
