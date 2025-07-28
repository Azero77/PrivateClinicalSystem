using ClinicApp.Domain.Common;

namespace ClinicApp.Domain.SessionAgg
{
    public static class SessionDomainEventFactory
    {
        public static IDomainEvent From(SessionState state) =>
            state.metadata switch
            {
                SetSessionMetadata m => new SessionSetEvent(m),
                SessionCreatedMetadata m => new SessionCreatedEvent(new SessionCreatedMetadata()),
                UpdatedSessionMetadata m => new SessionUpdatedEvent(m),
                DeletedSessionMetadata m => new SessionDeletedEvent(m),
                RejectedSessionMetadata m => new SessionRejectedEvent(m),
                StartedSessionMetadata m => new SessionStartedEvent(m),
                FinishedSessionMetadata m => new SessionFinishedEvent(m),
                _ => throw new NotSupportedException($"Unknown metadata type: {state.metadata.GetType().Name}")
            };
    }

    public record SessionCreatedEvent(SessionCreatedMetadata session) : IDomainEvent;
    public record SessionSetEvent(SetSessionMetadata Metadata) : IDomainEvent;

    public record SessionUpdatedEvent(UpdatedSessionMetadata Metadata) : IDomainEvent;

    public record SessionDeletedEvent(DeletedSessionMetadata Metadata) : IDomainEvent;

    public record SessionRejectedEvent(RejectedSessionMetadata Metadata) : IDomainEvent;
    public record SessionStartedEvent(StartedSessionMetadata Metadata) : IDomainEvent;

    public record SessionFinishedEvent(FinishedSessionMetadata Metadata) : IDomainEvent;

}
