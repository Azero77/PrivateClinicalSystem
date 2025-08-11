
using ClinicApp.Application.Queries;
using ClinicApp.Application.QueryServices;
using MediatR;
using Microsoft.AspNetCore.Mvc;

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
        if (query.IsError)
            return NotFound();
        return Ok(query.Value);
    }
}
