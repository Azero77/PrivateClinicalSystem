using ClinicApp.Presentation.Extensions;
using ErrorOr;
using Microsoft.AspNetCore.Mvc;

namespace ClinicApp.Presentation.Controllers;

public abstract class ApiController : ControllerBase
{

    internal IActionResult ProblemResult(List<Error> errors)
    {
        return errors.ToProblemDetails().ToProblemResult(HttpContext);
    }
}