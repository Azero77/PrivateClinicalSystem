using System.Net;
using ErrorOr;
using Microsoft.AspNetCore.Mvc;

namespace API;

public static class ProblemResult
{
    public static IResult Create(List<Error> errors)
    {
        if (!errors.Any())
        {
            // This should not happen
            return Results.Problem(statusCode: (int)HttpStatusCode.InternalServerError, title: "An unexpected error occurred.");
        }
        
        var firstError = errors[0];

        var statusCode = firstError.Type switch
        {
            ErrorType.Conflict => (int)HttpStatusCode.Conflict,
            ErrorType.Validation => (int)HttpStatusCode.BadRequest,
            ErrorType.NotFound => (int)HttpStatusCode.NotFound,
            ErrorType.Unauthorized => (int)HttpStatusCode.Unauthorized,
            _ => (int)HttpStatusCode.InternalServerError
        };

        return Results.Problem(statusCode: statusCode, title: firstError.Code, extensions: new Dictionary<string, object?>()
        {
            {"errors",errors}
        });

    }
}
