using ClinicApp.Domain.SessionAgg;
using MediatR;

namespace ClinicApp.Application.EventHandlers;
public class SessionModifiedEventHandler
    <TDomainEvent>
    : INotificationHandler<TDomainEvent>
    where TDomainEvent : SessionDomainEvent
{
    private readonly IEventAdderService<SessionDomainEvent> _eventAdderService;

    public SessionModifiedEventHandler(IEventAdderService<SessionDomainEvent> eventAdderService)
    {
        _eventAdderService = eventAdderService;
    }

    public Task Handle(TDomainEvent notification, CancellationToken cancellationToken)
    {
        var sessionId = notification.sessionId;
        _eventAdderService.Add(notification);
        return Task.CompletedTask;
    }
}
