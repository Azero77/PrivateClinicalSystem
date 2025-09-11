using ClinicApp.Application.Common;
using ClinicApp.Domain.Repositories;
using ClinicApp.Domain.SessionAgg;
using ErrorOr;
using MediatR;

namespace ClinicApp.Application.Commands.ModifySession;

public abstract class ModifySessionCommandHandler<TCommand> : IRequestHandler<TCommand, ErrorOr<Success>>
    where TCommand : ModifySessionCommand
{
    private readonly ISessionRepository _repo;
    private readonly IUnitOfWork _unitOfWork;

    protected ModifySessionCommandHandler(ISessionRepository repo, IUnitOfWork unitOfWork)
    {
        _repo = repo;
        _unitOfWork = unitOfWork;
    }

    protected abstract Task<IErrorOr> ApplySessionAction(Session session,TCommand command);

    public async Task<ErrorOr<Success>> Handle(TCommand command, CancellationToken cancellationToken)
    {
        var session = await _repo.GetById(command.SessionId);
        if (session is null)
            return Error.NotFound();


        var result = await ApplySessionAction(session,command);
        if (result.IsError)
            return result.Errors!;
        await _repo.SaveAsync(session);
        await _unitOfWork.SaveChangesAsync(cancellationToken,session);
        return Result.Success;
    }
}