using ClinicApp.Domain.Common;
using ClinicApp.Domain.Common.ValueObjects;
using ClinicApp.Domain.SessionAgg;
using ErrorOr;

namespace ClinicApp.Domain.Services.Sessions
{
    public interface ISessionScheduler<T> where T : Entity
    {
        Task<ErrorOr<Created>> CreateSession(Session session,T entity);
        ErrorOr<Success> SetSession(Session session, T entity);
        Task<ErrorOr<Updated>> UpdateSession(Session session, TimeRange newTime, T entity);
        Task<ErrorOr<Deleted>> DeleteSession(Session session, T entity);
        ErrorOr<Success> PaySession(Session session, T entity);
    }
}
