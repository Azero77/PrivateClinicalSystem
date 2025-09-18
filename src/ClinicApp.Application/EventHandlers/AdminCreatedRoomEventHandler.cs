using ClinicApp.Domain.AdminAggregate;
using MediatR;

namespace ClinicApp.Application.NotificationHandlers;

public class AdminCreatedRoomEventHandler : INotificationHandler<AdminCreatedRoomEvent>
{
    public Task Handle(AdminCreatedRoomEvent notification, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}