using ClinicApp.Domain.Common.ValueObjects;

namespace ClinicApp.Domain.SessionAgg
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
        public static SessionState SetSessionState(DateTimeOffset setTimeAt) => new SessionState(SessionStatus.Set, new SetSessionMetadata(setTimeAt));
        public static SessionState UpdatedSessionState(TimeRange oldValue, TimeRange newValue, DateTimeOffset updateAt) =>
       new(SessionStatus.Updated, new UpdatedSessionMetadata(oldValue, newValue, updateAt));

        public static SessionState DeletedSessionState(DateTimeOffset deletedAt) =>
            new(SessionStatus.Deleted, new DeletedSessionMetadata(deletedAt));

        public static SessionState RejectedSessionState(DateTimeOffset rejectedAt, string? execuse = null) =>
            new(SessionStatus.Rejected, new RejectedSessionMetadata(rejectedAt, execuse));

        public static SessionState StartedSessionState(DateTimeOffset startedAt) =>
        new(SessionStatus.Started, new StartedSessionMetadata(startedAt));

        public static SessionState FinishedSessionState(DateTimeOffset finishedAt) =>
            new(SessionStatus.Finished, new FinishedSessionMetadata(finishedAt));
    };


    public abstract record SessionStateMetaData;
    public record SessionCreatedMetadata : SessionStateMetaData;
    public record UpdatedSessionMetadata(TimeRange oldValue,TimeRange newValue,DateTimeOffset updateAt) : SessionStateMetaData;
    public record DeletedSessionMetadata(DateTimeOffset deletedAt) : SessionStateMetaData;
    public record SetSessionMetadata(DateTimeOffset setTimeAt) : SessionStateMetaData;
    public record RejectedSessionMetadata(DateTimeOffset RejectedAt,string? execuse = null) : SessionStateMetaData;
    public record StartedSessionMetadata(DateTimeOffset StartedAt) : SessionStateMetaData;
    public record FinishedSessionMetadata(DateTimeOffset FinishedAt) : SessionStateMetaData;
}


