using ErrorOr;
using ClinicApp.Domain.Session;
using ClinicApp.Domain.Common.ValueObjects;
namespace ClinicApp.Domain.Services;

public interface ISessionStateModifier
{
    ErrorOr<Created> CreateSession(Session.Session session);
    ErrorOr<Success> SetSession(Session.Session session);
    ErrorOr<Updated> UpdateSession(Session.Session session, TimeRange newTime);
    ErrorOr<Deleted> DeleteSession(Session.Session session);
    ErrorOr<Success> PaySession(Session.Session session);
}
