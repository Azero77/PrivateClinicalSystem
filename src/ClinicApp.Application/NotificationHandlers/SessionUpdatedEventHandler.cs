using ClinicApp.Domain.SessionAgg;
using MediatR;

namespace ClinicApp.Application.NotificationHandlers;

public class SessionUpdatedEventHandler : INotificationHandler<SessionUpdatedEvent>
{
    public Task Handle(SessionUpdatedEvent notification, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}