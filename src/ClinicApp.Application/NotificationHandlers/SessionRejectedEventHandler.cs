using ClinicApp.Domain.SessionAgg;
using MediatR;

namespace ClinicApp.Application.NotificationHandlers;

public class SessionRejectedEventHandler : INotificationHandler<SessionRejectedEvent>
{
    public Task Handle(SessionRejectedEvent notification, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
