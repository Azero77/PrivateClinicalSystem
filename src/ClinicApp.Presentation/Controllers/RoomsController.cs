using ClinicApp.Application.Commands.RoomCommands;
using ClinicApp.Application.Queries.Rooms;
using ClinicApp.Presentation.Requests;
using ClinicApp.Shared;
using ClinicApp.Shared.QueryTypes;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClinicApp.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RoomsController : ApiController
{
    private readonly IMediator _mediator;

    public RoomsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    [Authorize(Policy = PoliciesConstants.CanManageRooms)]
    public async Task<IActionResult> CreateRoom([FromBody] CreateRoomRequest request, CancellationToken cancellationToken)
    {
        var command = new CreateRoomCommand(request.Name);
        var result = await _mediator.Send(command, cancellationToken);
        return result.Match(
            room => CreatedAtAction(nameof(GetRoom), new { id = room.Id }, room),
            ProblemResult);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Policy = PoliciesConstants.CanManageRooms)]
    public async Task<IActionResult> UpdateRoom(Guid id, [FromBody] UpdateRoomRequest request, CancellationToken cancellationToken)
    {
        var command = new UpdateRoomCommand(id, request.Name);
        var result = await _mediator.Send(command, cancellationToken);
        return result.Match(
            room => Ok(room),
            ProblemResult);
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Policy = PoliciesConstants.CanManageRooms)]
    public async Task<IActionResult> DeleteRoom(Guid id, CancellationToken cancellationToken)
    {
        var command = new DeleteRoomCommand(id);
        var result = await _mediator.Send(command, cancellationToken);
        return result.Match(
            _ => NoContent(),
            ProblemResult);
    }

    [HttpGet("{id:guid}")]
    [Authorize(Policy = PoliciesConstants.CanViewRooms)]
    public async Task<IActionResult> GetRoom(Guid id, CancellationToken cancellationToken)
    {
        var query = new GetRoomByIdQuery(id);
        var result = await _mediator.Send(query, cancellationToken);
        return result.Match(Ok, ProblemResult);
    }

    [HttpGet]
    [Authorize(Policy = PoliciesConstants.CanViewRooms)]
    public async Task<IActionResult> GetRooms(CancellationToken cancellationToken)
    { 
        var query = new GetAllRoomsQuery();
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }
}
