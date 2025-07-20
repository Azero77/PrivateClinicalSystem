using ClinicApp.Domain.Common;
using ClinicApp.Domain.Common.ValueObjects;
using ErrorOr;

namespace ClinicApp.Domain.Services.Sessions
{
    public interface ISessionScheduler<T> where T : Entity
    {
        Task<ErrorOr<Created>> CreateSession(Session.Session session,T entity);
        ErrorOr<Success> SetSession(Session.Session session, T entity);
        Task<ErrorOr<Updated>> UpdateSession(Session.Session session, TimeRange newTime, T entity);
        ErrorOr<Deleted> DeleteSession(Session.Session session, T entity);
        ErrorOr<Success> PaySession(Session.Session session, T entity);
    }
}
