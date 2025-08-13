
using ClinicApp.Application.Queries;
using ClinicApp.Presentation.Extensions;
using ErrorOr;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace ClinicApp.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DoctorsController : ControllerBase
{
    private readonly IMediator _mediator;
    public DoctorsController(IMediator mediator, IProblemDetailsService problemDetailsService)
    {
        _mediator = mediator;
        _problemDetailsService = problemDetailsService;
    }

    [HttpGet("{id}/with-sessions")]
    public async Task<IActionResult> GetDoctorWithSessions(Guid id)
    {
        var query = await _mediator.Send(new GetDoctorWithSessionsQuery(id));
        return query.Match(
            value => Ok(value),
            errors => Problem(errors.ToProblemDetails())
            );
    }
}
