using ClinicApp.Application.Commands.RoomCommands;
using ClinicApp.Application.DTOs;
using ClinicApp.Application.Queries.Rooms;
using ClinicApp.Domain.Common.Entities;
using ClinicApp.Presentation.Requests;
using ClinicApp.Shared;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace ClinicApp.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class RoomsController : ApiController
{
    private readonly IMediator _mediator;

    public RoomsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    [Authorize(Policy = PoliciesConstants.CanManageRooms)]
    [ProducesResponseType(typeof(Room), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
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
    [ProducesResponseType(typeof(Room), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
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
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
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
    [ProducesResponseType(typeof(RoomDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetRoom(Guid id, CancellationToken cancellationToken)
    {
        var query = new GetRoomByIdQuery(id);
        var result = await _mediator.Send(query, cancellationToken);
        return result.Match(Ok, ProblemResult);
    }

    [HttpGet]
    [Authorize(Policy = PoliciesConstants.CanViewRooms)]
    [ProducesResponseType(typeof(IEnumerable<RoomDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetRooms(CancellationToken cancellationToken)
    { 
        var query = new GetAllRoomsQuery();
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }
}
