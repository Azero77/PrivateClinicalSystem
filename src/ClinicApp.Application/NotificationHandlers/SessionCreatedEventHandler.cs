using ClinicApp.Domain.SessionAgg;
using MediatR;

namespace ClinicApp.Application.NotificationHandlers;
public class SessionCreatedEventHandler : INotificationHandler<SessionCreatedEvent>
{
    public Task Handle(SessionCreatedEvent notification, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
