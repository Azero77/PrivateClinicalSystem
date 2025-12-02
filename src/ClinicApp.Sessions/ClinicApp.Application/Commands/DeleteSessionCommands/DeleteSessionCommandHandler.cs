using ClinicApp.Application.Commands.ModifySession;
using ClinicApp.Application.Common;
using ClinicApp.Domain.Repositories;
using ClinicApp.Domain.SessionAgg;
using ErrorOr;
using MediatR;

namespace ClinicApp.Application.Commands.DeleteSessionCommands;

public class DeleteSessionCommandHandler : ModifySessionCommandHandler<DeleteSessionCommand>
{
    public DeleteSessionCommandHandler(ISessionRepository repo, IUnitOfWork unitOfWork) : base(repo, unitOfWork)
    {
    }

    protected override Task<IErrorOr> ApplySessionAction(Session session,DeleteSessionCommand command)
    {
        IErrorOr result = session.DeleteSession();
        return Task.FromResult(result);
    }
}

public record DeleteSessionCommand(Guid id) : ModifySessionCommand(id);