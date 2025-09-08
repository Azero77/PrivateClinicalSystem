using ClinicApp.Domain.DoctorAgg;
using ClinicApp.Domain.SessionAgg;
using ErrorOr;

namespace ClinicApp.Domain.Repositories;

public interface ISessionRepository : IRepository<Session>
{
    Task<IReadOnlyCollection<Session>> GetFutureSessionsDoctor(Doctor doctor);
    Task<IReadOnlyCollection<Session>> GetAllSessionsForDoctor(Doctor doctor);
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
