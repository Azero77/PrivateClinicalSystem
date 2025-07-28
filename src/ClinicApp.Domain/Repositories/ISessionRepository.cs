using ClinicApp.Domain.DoctorAgg;
using ClinicApp.Domain.SessionAgg;
using ErrorOr;

namespace ClinicApp.Domain.Repositories;

public interface ISessionRepository
{
    Task<IEnumerable<Session>> GetFutureSessionsDoctor(Doctor doctor);
    Task<IEnumerable<Session>> GetAllSessionsForDoctor(Doctor doctor);
    Task<ErrorOr<Session>> GetSessionById(Guid sessionId);
    Task<IEnumerable<Session>> GetSessionsForToday();
    Task<IEnumerable<Session>> GetSessionsForDoctorToday(Guid doctorid);
    Task<IEnumerable<Session>> GetSessionsForDay(DateOnly date);
    Task<ErrorOr<Created>> AddSession(Session session);
    Task<ErrorOr<Success>> SetSession(Session session);
    Task<ErrorOr<Updated>> UpdateSession(Session session);
    Task<ErrorOr<Success>> RejectSession(Session session);
    Task<ErrorOr<Deleted>> DeleteSession(Guid sessionId);
    Task<IEnumerable<SessionState>> GetSessionHistory(Guid sessionId);
    Task RemoveSessionFromDoctor(Session session);
}
