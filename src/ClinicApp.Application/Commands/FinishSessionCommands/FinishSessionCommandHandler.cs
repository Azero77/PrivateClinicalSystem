using ClinicApp.Application.Commands.ModifySession;
using ClinicApp.Application.Common;
using ClinicApp.Domain.Repositories;
using ClinicApp.Domain.SessionAgg;
using ErrorOr;
using MediatR;

namespace ClinicApp.Application.Commands.FinishSessionCommands;

public class FinishSessionCommandHandler : ModifySessionCommandHandler<FinishSessionCommand>
{
    public FinishSessionCommandHandler(ISessionRepository repo, IUnitOfWork unitOfWork) : base(repo, unitOfWork)
    {
    }

    protected override Task<IErrorOr> ApplySessionAction(Session session,FinishSessionCommand command)
    {
        IErrorOr result = session.FinishSession();
        return Task.FromResult(result);
    }
}

public record FinishSessionCommand(Guid Id) : ModifySessionCommand(Id); 