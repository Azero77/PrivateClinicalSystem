using ClinicApp.Presentation.Extensions;
using ErrorOr;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;

namespace ClinicApp.Presentation.Controllers;

public abstract class ApiController : ControllerBase
{

    internal IActionResult ProblemResult(List<ErrorOr.Error> errors)
    {
        return errors.ToProblemDetails().ToProblemResult(HttpContext);
    }

    internal IActionResult ProblemResult(List<ValidationFailure> errors)
    {
        return errors.ToProblemDetails().ToProblemResult(HttpContext);
    }
}