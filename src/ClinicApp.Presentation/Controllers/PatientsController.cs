using ClinicApp.Application.Commands.PatientCommands;
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
public class PatientsController : ApiController
{
    private readonly IMediator _mediator;

    public PatientsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    [Authorize(Policy = PoliciesConstants.CanManageUsers)]
    public async Task<IActionResult> CreatePatient([FromBody] CreatePatientRequest request, CancellationToken cancellationToken)
    {
        var command = new CreatePatientCommand(request.FirstName, request.LastName, request.UserId);
        var result = await _mediator.Send(command, cancellationToken);
        return result.Match(
            patient => CreatedAtAction(nameof(GetPatient), new { id = patient.Id }, patient),
            ProblemResult);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Policy = PoliciesConstants.CanManageUsers)]
    public async Task<IActionResult> UpdatePatient(Guid id, [FromBody] UpdatePatientRequest request, CancellationToken cancellationToken)
    {
        var command = new UpdatePatientCommand(id, request.FirstName, request.LastName);
        var result = await _mediator.Send(command, cancellationToken);
        return result.Match(
            patient => Ok(patient),
            ProblemResult);
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Policy = PoliciesConstants.CanManageUsers)]
    public async Task<IActionResult> DeletePatient(Guid id, CancellationToken cancellationToken)
    {
        var command = new DeletePatientCommand(id);
        var result = await _mediator.Send(command, cancellationToken);
        return result.Match(
            _ => NoContent(),
            ProblemResult);
    }

    [HttpGet("{id:guid}")]
    [Authorize]
    public async Task<IActionResult> GetPatient(Guid id, CancellationToken cancellationToken)
    {
        var query = new QuerySingleRequest<PatientQueryType>(id);
        var result = await _mediator.Send(query, cancellationToken);
        return result is not null ? Ok(result) : NotFound();
    }

    [HttpGet]
    [Authorize(Policy = PoliciesConstants.CanManageUsers)]
    public async Task<IActionResult> GetPatients(CancellationToken cancellationToken)
    {
        var query = new QueryRequest<PatientQueryType>();
        var result = (await _mediator.Send(query, cancellationToken)).ToList();
        return Ok(result);
    }
}
