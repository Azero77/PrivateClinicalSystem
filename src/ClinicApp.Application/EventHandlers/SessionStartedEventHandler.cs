using ClinicApp.Domain.SessionAgg;
using MediatR;

namespace ClinicApp.Application.NotificationHandlers;

public class SessionStartedEventHandler : INotificationHandler<SessionStartedEvent>
{
    public Task Handle(SessionStartedEvent notification, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}