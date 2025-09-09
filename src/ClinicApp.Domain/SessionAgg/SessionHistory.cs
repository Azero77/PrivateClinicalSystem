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
        SessionStateMetaData metadata)
    {
        public static SessionState CreateSessionState(Session session) => new SessionState(session.SessionStatus, new SessionCreatedMetadata(session));
        public static SessionState SetSessionState(Guid sessionId,DateTimeOffset setTimeAt) => new SessionState(SessionStatus.Set, new SetSessionMetadata(sessionId,setTimeAt));
        public static SessionState UpdatedSessionState(Guid sessionId,TimeRange oldValue, TimeRange newValue, DateTimeOffset updateAt) =>
       new(SessionStatus.Updated, new UpdatedSessionMetadata(sessionId,oldValue, newValue, updateAt));

        public static SessionState DeletedSessionState(Guid sessionId,DateTimeOffset deletedAt) =>
            new(SessionStatus.Deleted, new DeletedSessionMetadata(sessionId,deletedAt));

        public static SessionState RejectedSessionState(Guid sessionId,DateTimeOffset rejectedAt, string? execuse = null) =>
            new(SessionStatus.Rejected, new RejectedSessionMetadata(sessionId,rejectedAt, execuse));

        public static SessionState StartedSessionState(Guid sessionId,DateTimeOffset startedAt) =>
        new(SessionStatus.Started, new StartedSessionMetadata(sessionId,startedAt));

        public static SessionState FinishedSessionState(Guid sessionId,DateTimeOffset finishedAt) =>
            new(SessionStatus.Finished, new FinishedSessionMetadata(sessionId,finishedAt));
    };


    public abstract record SessionStateMetaData;
    public record SessionCreatedMetadata(Session session) : SessionStateMetaData;
    public record UpdatedSessionMetadata(Guid SessionId,TimeRange OldValue,TimeRange NewValue,DateTimeOffset UpdateAt) : SessionStateMetaData;
    public record DeletedSessionMetadata(Guid SessionId,DateTimeOffset DeletedAt) : SessionStateMetaData;
    public record SetSessionMetadata(Guid SessionId,DateTimeOffset SetTimeAt) : SessionStateMetaData;
    public record RejectedSessionMetadata(Guid SessionId,DateTimeOffset RejectedAt,string? Execuse = null) : SessionStateMetaData;
    public record StartedSessionMetadata(Guid SessionId,DateTimeOffset StartedAt) : SessionStateMetaData;
    public record FinishedSessionMetadata(Guid SessionId,DateTimeOffset FinishedAt) : SessionStateMetaData;
}


