using ClinicApp.Domain.Common;
using ClinicApp.Domain.Common.ValueObjects;

namespace ClinicApp.Domain.SessionAgg
{
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
    }

    public record SessionState(SessionStatus newstate,
        SessionDomainEvent metadata)
    {
        public static SessionState CreateSessionState(Session session) => new SessionState(session.SessionStatus, new SessionCreatedMetadata(session));
        public static SessionState SetSessionState(Guid sessionId,DateTimeOffset setTimeAt) => new SessionState(SessionStatus.Set, new SetSessionMetadata(sessionId,setTimeAt));
        public static SessionState UpdatedSessionState(Guid sessionId,TimeRange oldValue, TimeRange newValue, DateTimeOffset updateAt) =>
       new(SessionStatus.Updated, new UpdatedSessionDomainEvent(sessionId,oldValue, newValue, updateAt));

        public static SessionState DeletedSessionState(Guid sessionId,DateTimeOffset deletedAt) =>
            new(SessionStatus.Deleted, new DeletedSessionDomainEvent(sessionId,deletedAt));

        public static SessionState RejectedSessionState(Guid sessionId,DateTimeOffset rejectedAt, string? execuse = null) =>
            new(SessionStatus.Rejected, new RejectedSessionDomainEvent(sessionId,rejectedAt, execuse));

        public static SessionState StartedSessionState(Guid sessionId,DateTimeOffset startedAt) =>
        new(SessionStatus.Started, new StartedSessionDomainEvent(sessionId,startedAt));

        public static SessionState FinishedSessionState(Guid sessionId,DateTimeOffset finishedAt) =>
            new(SessionStatus.Finished, new FinishedSessionDomainEvent(sessionId,finishedAt));
    };


    public abstract record SessionDomainEvent : IDomainEvent;
    public record SessionCreatedDomainEvent(Session session) : SessionDomainEvent;
    public record UpdatedSessionDomainEvent(Guid SessionId,TimeRange OldValue,TimeRange NewValue,DateTimeOffset UpdateAt) : SessionDomainEvent;
    public record DeletedSessionDomainEvent(Guid SessionId,DateTimeOffset DeletedAt) : SessionDomainEvent;
    public record SetSessionDomainEvent(Guid SessionId,DateTimeOffset SetTimeAt) : SessionDomainEvent;
    public record RejectedSessionDomainEvent(Guid SessionId,DateTimeOffset RejectedAt,string? Execuse = null) : SessionDomainEvent;
    public record StartedSessionDomainEvent(Guid SessionId,DateTimeOffset StartedAt) : SessionDomainEvent;
    public record FinishedSessionDomainEvent(Guid SessionId,DateTimeOffset FinishedAt) : SessionDomainEvent;
}


