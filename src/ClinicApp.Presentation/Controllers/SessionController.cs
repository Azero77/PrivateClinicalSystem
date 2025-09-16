﻿using ClinicApp.Application.Commands.AddSessionsCommands;
using ClinicApp.Application.Commands.DeleteSessionCommands;
using ClinicApp.Application.Commands.FinishSessionCommands;
using ClinicApp.Application.Commands.RejectSessionsCommands;
using ClinicApp.Application.Commands.SetSessionsCommands;
using ClinicApp.Application.Commands.StartSessionCommands;
using ClinicApp.Application.Commands.UpdateSessionDateCommands;
using ClinicApp.Application.Queries.Common;
using ClinicApp.Application.Queries.Sessions.SessionHistory;
using ClinicApp.Domain.Common;
using ClinicApp.Presentation.Authorization.Filters;
using ClinicApp.Presentation.Helpers;
using ClinicApp.Presentation.Requests;
using ClinicApp.Shared;
using ClinicApp.Shared.QueryTypes;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace ClinicApp.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public partial class SessionController : ApiController
{
    private readonly IMediator _mediator;

    public SessionController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("add")]
    [AuthorizeByRequestFilter<CanAddSessionRequirement,AddSessionRequest>] //admins and secretary can add any session,doctor can add session of their own
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
    [Authorize(Policy = PoliciesConstants.CanViewOwnSessionsPolicy)]
    public async Task<IActionResult> GetSession(
        [FromRoute] Guid id)
    {
        var query = new QuerySingleRequest<SessionQueryType>(id);
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("sessions/history/{id}")]
    [Authorize(Policy = PoliciesConstants.CanViewSessionHistory)]
    public async Task<IActionResult> SessionHistory(
        [FromRoute] GetSessionHistoryRequest request)
    {
        GetSessionHistoryQuery query = new(request.id);
        var result = await _mediator.Send(query);
        return result.Match(value => Ok(value),errors => ProblemResult(errors));
    }
    [HttpDelete("sessions/{id}/delete")]
    [Authorize(Policy = PoliciesConstants.CanDeleteSession)]
    public async Task<IActionResult> SessionDelete(
        [FromRoute] ModifySessionRequest request)
    {
        var command = new DeleteSessionCommand(request.id);
        var result = await _mediator.Send(command);
        return result.Match(value => NoContent(),ProblemResult);
    }
    [HttpPatch("sessions/{id}/reject")]
    [AuthorizeByRequestFilter<CanUpdateSessionRequirement,ModifySessionRequest>]
    public async Task<IActionResult> SessionReject(
        [FromRoute] ModifySessionRequest request)
    {
        var command = new RejectSessionCommand(request.id);
        var result = await _mediator.Send(command);
        return result.Match(value => NoContent(), ProblemResult);
    }
    [HttpPatch("sessions/{id}/finish")]
    [AuthorizeByRequestFilter<CanUpdateSessionRequirement, ModifySessionRequest>]
    public async Task<IActionResult> SessionFinish(
        [FromRoute] ModifySessionRequest request)
    {
        var command = new FinishSessionCommand(request.id);
        var result = await _mediator.Send(command);
        return result.Match(value => NoContent(), ProblemResult);
    }
    [HttpPatch("sessions/{id}/set")]
    [AuthorizeByRequestFilter<CanUpdateSessionRequirement, ModifySessionRequest>]
    public async Task<IActionResult> SessionSet(
        [FromRoute] ModifySessionRequest request)
    {
        var command = new SetSessionCommand(request.id);
        var result = await _mediator.Send(command);
        return result.Match(value => NoContent(), ProblemResult);
    }

    [HttpPatch("sessions/{id}/start")]
    [AuthorizeByRequestFilter<CanUpdateSessionRequirement, ModifySessionRequest>]
    public async Task<IActionResult> SessionStart(
        [FromRoute] ModifySessionRequest request)
    {
        var command = new StartSessionCommand(request.id);
        var result = await _mediator.Send(command);
        return result.Match(value => NoContent(), ProblemResult);
    }

    [HttpPatch("sessions/{id}/update-time")]
    [AuthorizeByRequestFilter<CanUpdateSessionRequirement, ModifySessionRequest>]
    public async Task<IActionResult> SessionUpdateTime(
        [FromRoute] UpdateSessionTimeRequest request)
    {
        var command = new UpdateSessionDateCommand(request.SessionId,request.StartTime,request.EndTime);
        var result = await _mediator.Send(command);
        return result.Match(value => NoContent(), ProblemResult);
    }

}
