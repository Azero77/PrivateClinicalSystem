using ClinicApp.Application.Commands.AddSessionsCommands;
using ClinicApp.Application.Commands.DeleteSessionCommands;
using ClinicApp.Application.Commands.FinishSessionCommands;
using ClinicApp.Application.Commands.ModifySession;
using ClinicApp.Application.Commands.RejectSessionsCommands;
using ClinicApp.Application.Commands.SetSessionsCommands;
using ClinicApp.Application.Commands.StartSessionCommands;
using ClinicApp.Application.Commands.UpdateSessionDateCommands;
using ClinicApp.Application.Queries.Common;
using ClinicApp.Application.Queries.Sessions;
using ClinicApp.Application.Queries.Sessions.SessionHistory;
using ClinicApp.Application.QueryTypes;
using ClinicApp.Domain.Common;
using ClinicApp.Domain.SessionAgg;
using ClinicApp.Presentation.Helpers;
using ClinicApp.Presentation.Requests;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace ClinicApp.Presentation.Controllers;

[Route("api/[controller]")]
[Produces("application/json")]
[ApiController]
public partial class SessionController : ApiController
{
    private readonly IMediator _mediator;

    public SessionController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("add")]
    [Authorize]
    public async Task<IActionResult> AddSession(
        [FromBody]
        AddSessionRequest request,CancellationToken token)
    {
        UserRole userRole = HttpContext.User.GetRole();
        var command = new AddSessionCommand(request.StartTime,
                                            request.EndTime,
                                            request.SessionDescriptionContent,
                                            request.RoomId,
                                            request.PatientId,
                                            request.DoctorId,
                                            userRole);

        var result = await _mediator.Send(command,token);
        if (!result.IsError)
        {
            Log.Debug("Added {Session} In {TimeStamp}",new object[] { result.Value,DateTime.UtcNow});
        }
        return result.Match(value => 
            CreatedAtAction(
            nameof(GetSession),
            new { id = value.Id },
            value),
            ProblemResult);
    }

    [HttpGet("sessions/{id}")]
    public async Task<IActionResult> GetSession(
        [FromRoute] Guid id)
    {
        var query = new QuerySingleRequest<SessionQueryType>(id);
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("sessions/history/{id}")]
    public async Task<IActionResult> SessionHistory(
        [FromRoute] GetSessionHistoryRequest request)
    {
        GetSessionHistoryQuery query = new(request.id);
        var result = await _mediator.Send(query);
        return result.Match(value => Ok(value),errors => ProblemResult(errors));
    }
    [HttpDelete("sessions/{id}/delete")]
    public async Task<IActionResult> SessionDelete(
        [FromRoute] ModifySessionRequest request)
    {
        var command = new DeleteSessionCommand(request.id);
        var result = await _mediator.Send(command);
        return result.Match(value => NoContent(),ProblemResult);
    }
    [HttpPatch("sessions/{id}/reject")]
    public async Task<IActionResult> SessionReject(
        [FromRoute] ModifySessionRequest request)
    {
        var command = new RejectSessionCommand(request.id);
        var result = await _mediator.Send(command);
        return result.Match(value => NoContent(), ProblemResult);
    }
    [HttpPatch("sessions/{id}/finish")]
    public async Task<IActionResult> SessionFinish(
        [FromRoute] ModifySessionRequest request)
    {
        var command = new FinishSessionCommand(request.id);
        var result = await _mediator.Send(command);
        return result.Match(value => NoContent(), ProblemResult);
    }
    [HttpPatch("sessions/{id}/set")]
    public async Task<IActionResult> SessionSet(
        [FromRoute] ModifySessionRequest request)
    {
        var command = new SetSessionCommand(request.id);
        var result = await _mediator.Send(command);
        return result.Match(value => NoContent(), ProblemResult);
    }

    [HttpPatch("sessions/{id}/start")]
    public async Task<IActionResult> SessionStart(
        [FromRoute] ModifySessionRequest request)
    {
        var command = new StartSessionCommand(request.id);
        var result = await _mediator.Send(command);
        return result.Match(value => NoContent(), ProblemResult);
    }

    [HttpPatch("sessions/{id}/update-time")]
    public async Task<IActionResult> SessionUpdateTime(
        [FromRoute] UpdateSessionTimeRequest request)
    {
        var command = new UpdateSessionDateCommand(request.SessionId,request.StartTime,request.EndTime);
        var result = await _mediator.Send(command);
        return result.Match(value => NoContent(), ProblemResult);
    }

}
