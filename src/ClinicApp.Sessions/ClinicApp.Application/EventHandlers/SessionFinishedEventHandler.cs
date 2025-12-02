using ClinicApp.Application.EventHandlers;
using ClinicApp.Domain.SessionAgg;
using MediatR;

namespace ClinicApp.Application.NotificationHandlers;

public class SessionFinishedEventHandler : SessionModifiedEventHandler<FinishedSessionDomainEvent>
{
    public SessionFinishedEventHandler(IEventAdderService<SessionDomainEvent> eventAdderService) : base(eventAdderService)
    {
    }
}