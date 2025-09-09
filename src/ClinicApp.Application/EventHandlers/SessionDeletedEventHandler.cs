using ClinicApp.Application.Common;
using ClinicApp.Application.EventHandlers;
using ClinicApp.Domain.Repositories;
using ClinicApp.Domain.SessionAgg;
using MediatR;

namespace ClinicApp.Application.NotificationHandlers;

public class SessionDeletedEventHandler : SessionModifiedEventHandler<DeletedSessionDomainEvent>
{
    public SessionDeletedEventHandler(IEventAdderService<SessionDomainEvent> eventAdderService) : base(eventAdderService)
    {
    }
}