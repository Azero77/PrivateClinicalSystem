using ClinicApp.Application.EventHandlers;
using ClinicApp.Domain.SessionAgg;
using MediatR;

namespace ClinicApp.Application.NotificationHandlers;

public class SessionUpdatedEventHandler : SessionModifiedEventHandler<UpdatedSessionDomainEvent>
{
    public SessionUpdatedEventHandler(IEventAdderService<SessionDomainEvent> eventAdderService) : base(eventAdderService)
    {
    }
}