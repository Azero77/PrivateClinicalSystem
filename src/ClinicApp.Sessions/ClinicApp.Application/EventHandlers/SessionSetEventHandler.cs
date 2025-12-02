using ClinicApp.Application.EventHandlers;
using ClinicApp.Domain.SessionAgg;
using MediatR;

namespace ClinicApp.Application.NotificationHandlers;
public class SessionSetEventHandler : SessionModifiedEventHandler<SetSessionDomainEvent>
{
    public SessionSetEventHandler(IEventAdderService<SessionDomainEvent> eventAdderService) : base(eventAdderService)
    {
    }
}
