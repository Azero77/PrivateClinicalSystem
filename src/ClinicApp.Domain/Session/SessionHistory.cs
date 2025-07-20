using ClinicApp.Domain.Common.ValueObjects;

namespace ClinicApp.Domain.Session
{
    public class SessionHistory
    {
        public Queue<SessionState> States { get; private set; } = new Queue<SessionState>();
       
        public void AddNewState(SessionState state)
        {
            States.Append(state);
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
        public static SessionState SetSessionState(DateTime setTimeAt) => new SessionState(SessionStatus.Set, new SetSessionMetadata(setTimeAt));
        public static SessionState UpdatedSessionState(TimeRange oldValue, TimeRange newValue, DateTime updateAt) =>
       new(SessionStatus.Updated, new UpdatedSessionMetadata(oldValue, newValue, updateAt));

        public static SessionState DeletedSessionState(DateTime deletedAt) =>
            new(SessionStatus.Deleted, new DeletedSessionMetadata(deletedAt));

        public static SessionState RejectedSessionState(DateTime rejectedAt, string? execuse = null) =>
            new(SessionStatus.Rejected, new RejectedSessionMetadata(rejectedAt, execuse));

        public static SessionState StartedSessionState(DateTime startedAt) =>
        new(SessionStatus.Started, new StartedSessionMetadata(startedAt));

        public static SessionState FinishedSessionState(DateTime finishedAt) =>
            new(SessionStatus.Finished, new FinishedSessionMetadata(finishedAt));
    };


    public abstract record SessionStateMetaData;
    public record SessionCreatedMetadata : SessionStateMetaData;
    public record UpdatedSessionMetadata(TimeRange oldValue,TimeRange newValue,DateTime updateAt) : SessionStateMetaData;
    public record DeletedSessionMetadata(DateTime deletedAt) : SessionStateMetaData;
    public record SetSessionMetadata(DateTime setTimeAt) : SessionStateMetaData;
    public record RejectedSessionMetadata(DateTime RejectedAt,string? execuse = null) : SessionStateMetaData;
    public record StartedSessionMetadata(DateTime StartedAt) : SessionStateMetaData;
    public record FinishedSessionMetadata(DateTime FinishedAt) : SessionStateMetaData;
}


