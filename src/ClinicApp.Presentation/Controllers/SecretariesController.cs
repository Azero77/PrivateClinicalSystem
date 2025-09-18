using ClinicApp.Application.Commands.SecretaryCommands;
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
public class SecretariesController : ApiController
{
    private readonly IMediator _mediator;

    public SecretariesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    [Authorize(Policy = PoliciesConstants.CanManageUsers)]
    public async Task<IActionResult> CreateSecretary([FromBody] CreateSecretaryRequest request, CancellationToken cancellationToken)
    {
        var command = new CreateSecretaryCommand(request.FirstName, request.LastName, request.UserId);
        var result = await _mediator.Send(command, cancellationToken);
        return result.Match(
            secretary => CreatedAtAction(nameof(GetSecretary), new { id = secretary.Id }, secretary),
            ProblemResult);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Policy = PoliciesConstants.CanManageUsers)]
    public async Task<IActionResult> UpdateSecretary(Guid id, [FromBody] UpdateSecretaryRequest request, CancellationToken cancellationToken)
    {
        var command = new UpdateSecretaryCommand(id, request.FirstName, request.LastName);
        var result = await _mediator.Send(command, cancellationToken);
        return result.Match(
            secretary => Ok(secretary),
            ProblemResult);
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Policy = PoliciesConstants.CanManageUsers)]
    public async Task<IActionResult> DeleteSecretary(Guid id, CancellationToken cancellationToken)
    {
        var command = new DeleteSecretaryCommand(id);
        var result = await _mediator.Send(command, cancellationToken);
        return result.Match(
            _ => NoContent(),
            ProblemResult);
    }

    [HttpGet("{id:guid}")]
    [Authorize(Policy = PoliciesConstants.CanManageUsers)]
    public async Task<IActionResult> GetSecretary(Guid id, CancellationToken cancellationToken)
    {
        var query = new QuerySingleRequest<SecretaryQueryType>(id);
        var result = await _mediator.Send(query, cancellationToken);
        return result is not null ? Ok(result) : NotFound();
    }

    [HttpGet]
    [Authorize(Policy = PoliciesConstants.CanManageUsers)]
    public async Task<IActionResult> GetSecretaries(CancellationToken cancellationToken)
    {
        var query = new QueryRequest<SecretaryQueryType>();
        var result = (await _mediator.Send(query, cancellationToken)).ToList();
        return Ok(result);
    }
}
