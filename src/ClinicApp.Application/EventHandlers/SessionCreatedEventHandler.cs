using ClinicApp.Application.EventHandlers;
using ClinicApp.Domain.SessionAgg;
using MediatR;

namespace ClinicApp.Application.NotificationHandlers;
public class SessionCreatedEventHandler : SessionModifiedEventHandler<SessionCreatedDomainEvent>
{
    public SessionCreatedEventHandler(IEventAdderService<SessionDomainEvent> eventAdderService) : base(eventAdderService)
    {
    }
}
