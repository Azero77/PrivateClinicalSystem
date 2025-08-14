using ErrorOr;
using Microsoft.AspNetCore.Mvc;
using OpenTelemetry.Trace;

namespace ClinicApp.Presentation.Extensions;

public static class ErrorExtension
{
    public static ProblemDetails ToProblemDetails(this List<ErrorOr.Error> errors)
    {

        return new ProblemDetails()
        {
            Status = errors.Count switch
            {
                1 => errors.First().Type switch
                {
                    ErrorType.Failure => StatusCodes.Status400BadRequest,
                    ErrorType.Unexpected => StatusCodes.Status500InternalServerError,
                    ErrorType.Validation => StatusCodes.Status422UnprocessableEntity,
                    ErrorType.Conflict => StatusCodes.Status409Conflict,
                    ErrorType.NotFound => StatusCodes.Status404NotFound,
                    ErrorType.Unauthorized => StatusCodes.Status401Unauthorized,
                    ErrorType.Forbidden => StatusCodes.Status403Forbidden,
                    _ => StatusCodes.Status400BadRequest
                },
                _ => StatusCodes.Status400BadRequest
            },
            Extensions = new Dictionary<string, object?>
            {
                {"Errors",errors}
            }
        };
    }

    public static IActionResult ToProblemResult(this ProblemDetails problemDetails,HttpContext httpContext)
    {
        IProblemDetailsService service = httpContext.RequestServices.GetRequiredService<IProblemDetailsService>();

        service.WriteAsync(new ProblemDetailsContext()
        {
            HttpContext = httpContext,
            ProblemDetails = problemDetails
        });

        return new EmptyResult();
    }
}
