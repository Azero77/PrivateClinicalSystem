
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
    public DoctorsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("{id}/with-sessions")]
    public async Task<IActionResult> GetDoctorWithSessions(Guid id)
    {
        var query = await _mediator.Send(new GetDoctorWithSessionsQuery(id));
        return query.Match(
            Ok,
            errors => errors.ToProblemDetails().ToProblemResult(HttpContext)
            );
    }
}
