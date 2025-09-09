using ClinicApp.Domain.Common;
using ClinicApp.Domain.Common.ValueObjects;

namespace ClinicApp.Domain.SessionAgg
{/*
    public class SessionHistory
    {
        public Queue<SessionState> States { get; private set; } = new Queue<SessionState>();
       
        public void AddNewState(SessionState state)
        {
            States.Enqueue(state);
        }

        public void RemoveLastState()
        {
            States.Dequeue();
        }
    }*/

    public abstract record SessionDomainEvent(Guid sessionId) : IDomainEvent;
    public record SessionCreatedDomainEvent(Session session): SessionDomainEvent(session.Id);

    public record UpdatedSessionDomainEvent(
        Guid SessionId,
        TimeRange OldValue,
        TimeRange NewValue,
        DateTimeOffset UpdatedAt
    ) : SessionDomainEvent(SessionId);

    public record DeletedSessionDomainEvent(
        Guid SessionId,
        DateTimeOffset DeletedAt
    ) : SessionDomainEvent(SessionId);

    public record SetSessionDomainEvent(
        Guid SessionId,
        DateTimeOffset SetTimeAt
    ) : SessionDomainEvent(SessionId);

    public record RejectedSessionDomainEvent(
        Guid SessionId,
        DateTimeOffset RejectedAt,
        string? Excuse = null
    ) : SessionDomainEvent(SessionId);

    public record StartedSessionDomainEvent(
        Guid SessionId,
        DateTimeOffset StartedAt
    ) : SessionDomainEvent(SessionId);

    public record FinishedSessionDomainEvent(
        Guid SessionId,
        DateTimeOffset FinishedAt
    ) : SessionDomainEvent(SessionId);
}


