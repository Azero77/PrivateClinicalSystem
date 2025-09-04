using ClinicApp.Application.Commands.AddSessionsCommands;
using ClinicApp.Domain.Common;
using ClinicApp.Domain.Common.ValueObjects;
using ClinicApp.Presentation.Extensions;
using ErrorOr;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using System.Security.Claims;

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

    [HttpPost("Add")]
    [Authorize]
    public async Task<IActionResult> AddSession(
        [FromBody]
        AddSessionRequest request,CancellationToken token)
    {
        var validation = _validator.Validate(request);


        if (!validation.IsValid)
            return ProblemResult(validation.Errors);
        var userRole = Enum.Parse<UserRole>(HttpContext.User.FindFirst("role")!.Value.ToString());
        var command = new AddSessionCommand(
            TimeRange.Create(request.StartTime,request.EndTime).Value,
            request.SessionDescription,
            request.roomId,
            request.patientId,
            request.doctorId,
            userRole
            );

        var result = await _mediator.Send(command,token);
        if (!result.IsError)
        {
            Log.Debug("Added {Session} In {TimeStamp}",new object[] { result.Value,DateTime.UtcNow});
        }
        return result.Match(
            Ok,
            ProblemResult);
    }
}
