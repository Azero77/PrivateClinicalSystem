using ClinicApp.Application.Common;
using ClinicApp.Domain.Repositories;
using ClinicApp.Domain.SessionAgg;
using MediatR;

namespace ClinicApp.Application.NotificationHandlers;

public class SessionDeletedEventHandler : INotificationHandler<SessionDeletedEvent>
{
    ISessionRepository _repo;

    public SessionDeletedEventHandler(ISessionRepository repo)
    {
        _repo = repo;
    }

    public async Task Handle(SessionDeletedEvent notification, CancellationToken cancellationToken)
    {
        await _repo.DeleteSession(notification.Metadata.SessionId);
    }
}