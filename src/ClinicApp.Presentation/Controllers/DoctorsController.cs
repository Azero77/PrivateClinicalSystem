
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

}
