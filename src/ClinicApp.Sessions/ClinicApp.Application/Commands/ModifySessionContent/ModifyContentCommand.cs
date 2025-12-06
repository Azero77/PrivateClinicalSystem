using ClinicApp.Application.Commands.ModifySession;
using ClinicApp.Application.Common;
using ClinicApp.Domain.Repositories;
using ClinicApp.Domain.SessionAgg;
using ErrorOr;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ClinicApp.Application.Commands.ModifySessionContent;
public record ModifySessionContentCommand(Guid SessionId, JsonElement Content) : ModifySessionCommand(SessionId);

public sealed class ModifySessionContentCommandHandler(ISessionRepository repo,IUnitOfWork unitOfWork) : 
    ModifySessionCommandHandler<ModifySessionContentCommand>(repo,unitOfWork)
{
    protected override Task<IErrorOr> ApplySessionAction(Session session, ModifySessionContentCommand command)
    {
        IErrorOr result = session.SetDescription(new SessionDescription(command.Content));
        return Task.FromResult(result);
    }
}


public class ModifySessionContentCommandValidator : AbstractValidator<ModifySessionContentCommand>
{
    public ModifySessionContentCommandValidator()
    {
        RuleFor(x => x.SessionId)
            .NotEmpty();
    }
}