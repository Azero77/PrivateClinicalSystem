using ClinicApp.Application.Common;
using ClinicApp.Domain.Common.ValueObjects;
using ClinicApp.Domain.Repositories;
using ClinicApp.Domain.SessionAgg;
using ErrorOr;
using MediatR;

namespace ClinicApp.Application.Commands.UpdateSessionDateCommands;

public class UpdateSessionDateCommandHandler : IRequestHandler<UpdateSessionDateCommand, ErrorOr<Success>>
{
    private readonly ISessionRepository _repo;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateSessionDateCommandHandler(ISessionRepository repo, IUnitOfWork unitOfWork)
    {
        _repo = repo;
        _unitOfWork = unitOfWork;
    }

    public async Task<ErrorOr<Success>> Handle(UpdateSessionDateCommand request, CancellationToken cancellationToken)
    {
        var timerange = TimeRange.Create(request.StartTime, request.EndTime);
        if (timerange.IsError)
            return timerange.Errors;
        var session = await _repo.GetById(request.SessionId);
        if (session is null)
            return Errors.General.NotFound;
        var result = session.UpdateDate(timerange.Value);
        if (result.IsError)
            return result;
        await _repo.SaveAsync(session);
        await _unitOfWork.SaveChangesAsync(cancellationToken,session);
        return Result.Success;
    }
}