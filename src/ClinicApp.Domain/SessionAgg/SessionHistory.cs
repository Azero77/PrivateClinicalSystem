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
        public static SessionState CreateSessionState(SessionStatus initialStatus) => new SessionState(initialStatus, new SessionCreatedMetadata());
        public static SessionState SetSessionState(DateTimeOffset setTimeAt) => new SessionState(SessionStatus.Set, new SetSessionMetadata(setTimeAt));
        public static SessionState UpdatedSessionState(TimeRange oldValue, TimeRange newValue, DateTimeOffset updateAt) =>
       new(SessionStatus.Updated, new UpdatedSessionMetadata(oldValue, newValue, updateAt));

        public static SessionState DeletedSessionState(Guid sessionId,DateTimeOffset deletedAt) =>
            new(SessionStatus.Deleted, new DeletedSessionMetadata(sessionId,deletedAt));

        public static SessionState RejectedSessionState(DateTimeOffset rejectedAt, string? execuse = null) =>
            new(SessionStatus.Rejected, new RejectedSessionMetadata(rejectedAt, execuse));

        public static SessionState StartedSessionState(DateTimeOffset startedAt) =>
        new(SessionStatus.Started, new StartedSessionMetadata(startedAt));

        public static SessionState FinishedSessionState(DateTimeOffset finishedAt) =>
            new(SessionStatus.Finished, new FinishedSessionMetadata(finishedAt));
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


