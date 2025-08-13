using ClinicApp.Domain.SessionAgg;
using MediatR;

namespace ClinicApp.Application.NotificationHandlers;
public class SessionSetEventHandler : INotificationHandler<SessionSetEvent>
{
    public Task Handle(SessionSetEvent notification, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
