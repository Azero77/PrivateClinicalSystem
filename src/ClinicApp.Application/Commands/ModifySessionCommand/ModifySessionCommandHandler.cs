using ClinicApp.Application.Common;
using ClinicApp.Domain.Repositories;
using ClinicApp.Domain.SessionAgg;
using ErrorOr;
using MediatR;

namespace ClinicApp.Application.Commands.ModifySession;

public abstract class ModifySessionCommandHandler : IRequestHandler<ModifySessionCommand, ErrorOr<Success>>
{
    private readonly ISessionRepository _repo;
    private readonly IUnitOfWork _unitOfWork;

    protected ModifySessionCommandHandler(ISessionRepository repo, IUnitOfWork unitOfWork)
    {
        _repo = repo;
        _unitOfWork = unitOfWork;
    }

    protected abstract Task<IErrorOr> ApplySessionAction(Session session);

    public async Task<ErrorOr<Success>> Handle(ModifySessionCommand request, CancellationToken cancellationToken)
    {
        var session = await _repo.GetSessionById(request.SessionId);
        if (session is null)
            return Error.NotFound();


        var result = await ApplySessionAction(session);
        if (result.IsError)
            return result.Errors!;
        await _unitOfWork.SaveChangesAsync();

        return Result.Success;
    }
}