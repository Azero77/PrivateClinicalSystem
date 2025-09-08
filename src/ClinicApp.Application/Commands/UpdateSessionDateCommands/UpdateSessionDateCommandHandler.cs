using ClinicApp.Application.Common;
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
        var session = await _repo.GetById(request.SessionId);
        if (session is null)
            return Error.NotFound();
        var result = session.UpdateDate(request.NewTimeRange);
        if (result.IsError)
            return result;
        await _unitOfWork.SaveChangesAsync();
        return Result.Success;
    }
}