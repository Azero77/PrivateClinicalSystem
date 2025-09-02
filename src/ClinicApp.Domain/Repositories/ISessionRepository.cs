using ClinicApp.Domain.DoctorAgg;
using ClinicApp.Domain.SessionAgg;
using ErrorOr;

namespace ClinicApp.Domain.Repositories;

public interface ISessionRepository
{
    Task<IReadOnlyCollection<Session>> GetFutureSessionsDoctor(Guid doctorId);
    Task<IReadOnlyCollection<Session>> GetAllSessionsForDoctor(Guid doctorId);
    Task<Session?> GetSessionById(Guid sessionId);
    Task<IReadOnlyCollection<Session>> GetSessionsForToday();
    Task<IReadOnlyCollection<Session>> GetSessionsForDoctorToday(Guid doctorid);
    Task<IReadOnlyCollection<Session>> GetSessionsForDoctorOnDay(Guid doctorid,DateTimeOffset date);
    Task<IReadOnlyCollection<Session>>  GetSesssionsForDoctorOnDayAndAfter(Guid doctorid, DateTimeOffset date);
    Task<IReadOnlyCollection<Session>> GetSessionsForDay(DateTimeOffset date);
    Task<Session> AddSession(Session session);
    Task<Session> UpdateSession(Session session);
    Task<Session?> DeleteSession(Guid sessionId);
    Task<IReadOnlyCollection<SessionState>> GetSessionHistory(Guid sessionId);
}
