﻿using ClinicApp.Application.Commands.ModifySession;
using ClinicApp.Application.Common;
using ClinicApp.Domain.Repositories;
using ClinicApp.Domain.SessionAgg;
using ErrorOr;
using MediatR;

namespace ClinicApp.Application.Commands.SetSessionsCommands;

public class SetSessionCommandHandler : ModifySessionCommandHandler
{
    public SetSessionCommandHandler(ISessionRepository repo, IUnitOfWork unitOfWork) : base(repo, unitOfWork)
    {
    }

    protected override Task<IErrorOr> ApplySessionAction(Session session)
    {
        IErrorOr result = session.SetSession();
        return Task.FromResult(result);
    }
}
