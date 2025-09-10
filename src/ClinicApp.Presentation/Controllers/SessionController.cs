using ClinicApp.Application.Commands.AddSessionsCommands;
using ClinicApp.Application.Queries.Sessions.SessionHistory;
using ClinicApp.Domain.Common;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace ClinicApp.Presentation.Controllers;

public partial class SessionController : ApiController
{
    private readonly IMediator _mediator;

    public SessionController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("Add")]
    [Authorize]
    public async Task<IActionResult> AddSession(
        [FromBody]
        AddSessionRequest request,CancellationToken token)
    {
        var userRole = Enum.Parse<UserRole>(HttpContext.User.FindFirst("role")!.Value.ToString());
        var command = new AddSessionCommand(
            request.StartTime,
            request.EndTime,
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

    [HttpGet("Sessions/History/{id}")]
    public async Task<IActionResult> SessionHistory(
        [FromQuery] GetSessionHistoryRequest request)
    {
        GetSessionHistoryQuery query = new(request.id);
        var result = await _mediator.Send(query);
        return result.Match(value => Ok(value),errors => ProblemResult(errors));
    }
}
