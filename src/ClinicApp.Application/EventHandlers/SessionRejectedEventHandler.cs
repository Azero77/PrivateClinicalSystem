using ClinicApp.Application.EventHandlers;
using ClinicApp.Domain.SessionAgg;
using MediatR;

namespace ClinicApp.Application.NotificationHandlers;

public class SessionRejectedEventHandler : SessionModifiedEventHandler<RejectedSessionDomainEvent>
{
    public SessionRejectedEventHandler(IEventAdderService<SessionDomainEvent> eventAdderService) : base(eventAdderService)
    {
    }
}
