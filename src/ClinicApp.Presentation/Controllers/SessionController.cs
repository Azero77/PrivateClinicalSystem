using ClinicApp.Application.Commands.AddSessionsCommands;
using ClinicApp.Domain.Common;
using ClinicApp.Domain.Common.ValueObjects;
using ClinicApp.Presentation.Extensions;
using ErrorOr;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ClinicApp.Presentation.Controllers;

public class SessionController : ApiController
{
    private readonly IValidator<AddSessionRequest> _validator;
    private readonly IMediator _mediator;

    public SessionController(IValidator<AddSessionRequest> validator, IMediator mediator)
    {
        _validator = validator;
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> AddSession(AddSessionRequest request,CancellationToken token)
    {
        var validation = _validator.Validate(request);


        if (!validation.IsValid)
            throw new ApplicationException();

        var command = new AddSessionCommand(
            TimeRange.Create(request.StartTime,request.EndTime).Value,
            request.SessionDescription,
            request.roomId,
            request.patientId,
            request.doctorId,
            UserRole.Admin
            //detecting user role (should be changed later)
            );

        var result = await _mediator.Send(command,token);

        return result.Match(
            Ok,
            ProblemResult);
    }
}
