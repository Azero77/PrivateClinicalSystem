using ClinicApp.Domain.SessionAgg;
using MediatR;

namespace ClinicApp.Application.NotificationHandlers;

public class SessionDeletedEventHandler : INotificationHandler<SessionDeletedEvent>
{
    public Task Handle(SessionDeletedEvent notification, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}