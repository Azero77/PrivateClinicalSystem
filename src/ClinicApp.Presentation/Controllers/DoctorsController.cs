using ClinicApp.Application.Commands.DoctorAddCommands;
using ClinicApp.Application.Queries.Doctors;
using ClinicApp.Application.Queries.Common;
using ClinicApp.Presentation.Requests;
using ClinicApp.Shared;
using ClinicApp.Shared.QueryTypes;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClinicApp.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DoctorsController : ApiController
{
    private readonly IMediator _mediator;

    public DoctorsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    [Authorize(Policy = PoliciesConstants.CanManageUsers)]
    public async Task<IActionResult> CreateDoctor([FromBody] CreateDoctorRequest request, CancellationToken cancellationToken)
    {
        var command = new DoctorAddCommand(Guid.NewGuid(), request.UserId, request.RoomId, request.FirstName, request.LastName, request.WorkingDays, request.WorkingHoursStartTime, request.WorkingHoursEndTime, request.Major);
        var result = await _mediator.Send(command, cancellationToken);
        return result.Match(
            doctor => CreatedAtAction(nameof(GetDoctor), new { id = doctor.Id }, doctor),
            ProblemResult);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Policy = PoliciesConstants.CanManageUsers)]
    public async Task<IActionResult> UpdateDoctor(Guid id, [FromBody] UpdateDoctorRequest request, CancellationToken cancellationToken)
    {
        var command = new UpdateDoctorCommand(id, request.FirstName, request.LastName, request.RoomId, request.WorkingDays, request.WorkingHoursStartTime, request.WorkingHoursEndTime, request.Major);
        var result = await _mediator.Send(command, cancellationToken);
        return result.Match(
            doctor => Ok(doctor),
            ProblemResult);
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Policy = PoliciesConstants.CanManageUsers)]
    public async Task<IActionResult> DeleteDoctor(Guid id, CancellationToken cancellationToken)
    {
        var command = new DeleteDoctorCommand(id);
        var result = await _mediator.Send(command, cancellationToken);
        return result.Match(
            _ => NoContent(),
            ProblemResult);
    }

    [HttpGet("{id:guid}")]
    [Authorize(Policy = PoliciesConstants.CanViewDoctorsInfo)]
    public async Task<IActionResult> GetDoctor(Guid id, CancellationToken cancellationToken)
    {
        var query = new QuerySingleRequest<DoctorQueryType>(id);
        var result = await _mediator.Send(query, cancellationToken);
        return result is not null ? Ok(result) : NotFound();
    }

    [HttpGet]
    [Authorize(Policy = PoliciesConstants.CanViewDoctorsInfo)]
    public async Task<IActionResult> GetDoctors(CancellationToken cancellationToken)
    {
        var query = new GetDoctorsQuery();
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }
}