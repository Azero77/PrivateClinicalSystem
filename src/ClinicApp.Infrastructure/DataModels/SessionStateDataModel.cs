using ClinicApp.Domain.SessionAgg;
using System.Text.Json;

namespace ClinicApp.Infrastructure.DataModels;
internal record SessionState(Guid Id,
                            Guid SessionId,
                            string EventType,
                            string content,
                            DateTime eventDate //To Utc
    )
{
    public static SessionState From(Guid id,Guid sessionId,SessionDomainEvent sessionDomainEvent,DateTime eventDate)
    {
        return new SessionState(
            id,
            sessionId,
            sessionDomainEvent.GetType().Name,
            JsonSerializer.Serialize(sessionDomainEvent),
            eventDate
            );
    }
}