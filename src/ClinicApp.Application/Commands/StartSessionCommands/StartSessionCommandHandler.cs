using ClinicApp.Application.Commands.ModifySession;
using ClinicApp.Application.Common;
using ClinicApp.Domain.Repositories;
using ClinicApp.Domain.SessionAgg;
using ErrorOr;
using MediatR;

namespace ClinicApp.Application.Commands.StartSessionCommands;

public class StartSessionCommandHandler : ModifySessionCommandHandler
{
    public StartSessionCommandHandler(ISessionRepository repo, IUnitOfWork unitOfWork) : base(repo, unitOfWork)
    {
    }

    protected override Task<IErrorOr> ApplySessionAction(Session session)
    {
        IErrorOr result = session.StartSession();
        return Task.FromResult(result);
    }
}