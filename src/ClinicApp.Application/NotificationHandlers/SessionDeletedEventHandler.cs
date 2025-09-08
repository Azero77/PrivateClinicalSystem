using ClinicApp.Application.Common;
using ClinicApp.Domain.Repositories;
using ClinicApp.Domain.SessionAgg;
using MediatR;

namespace ClinicApp.Application.NotificationHandlers;

public class SessionDeletedEventHandler : INotificationHandler<SessionDeletedEvent>
{
    ISessionRepository _repo;
    IUnitOfWork _unitOfWork;

    public SessionDeletedEventHandler(ISessionRepository repo, IUnitOfWork unitOfWork)
    {
        _repo = repo;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(SessionDeletedEvent notification, CancellationToken cancellationToken)
    {
        await _repo.DeleteSession(notification.Metadata.sessionId);
        await _unitOfWork.SaveChangesAsync();
    }
}