
using ClinicApp.Application.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

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

    [HttpGet("{id}/with-sessions")]
    public async Task<IActionResult> GetDoctorWithSessions(Guid id)
    {
        var query = await _mediator.Send(new GetDoctorWithSessionsQuery(id));
        return query.Match(
            Ok,
            ProblemResult
            );
    }

}
