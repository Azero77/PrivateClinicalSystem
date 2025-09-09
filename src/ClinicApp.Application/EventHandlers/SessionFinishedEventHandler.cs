using ClinicApp.Domain.SessionAgg;
using MediatR;

namespace ClinicApp.Application.NotificationHandlers;

public class SessionFinishedEventHandler : INotificationHandler<SessionFinishedEvent>
{
    public Task Handle(SessionFinishedEvent notification, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}