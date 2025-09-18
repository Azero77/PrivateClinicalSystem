using ClinicApp.Domain.AdminAggregate;
using MediatR;

namespace ClinicApp.Application.NotificationHandlers;

public class AdminDeletedRoomEventHandler : INotificationHandler<AdminDeletedRoomEvent>
{
    public Task Handle(AdminDeletedRoomEvent notification, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}