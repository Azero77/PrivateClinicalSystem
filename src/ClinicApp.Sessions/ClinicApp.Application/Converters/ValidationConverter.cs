using ErrorOr;
using FluentValidation.Results;

namespace ClinicApp.Application.Converters;
public static class ValidationConverter
{
    public static List<Error> FromValidationToErrors(this IList<ValidationFailure> failures)
    {
        var result = new List<Error>();
        foreach (var failure in failures)
        {
            result.Add(Error.Validation(
                failure.ErrorCode,
                failure.ErrorMessage,
                new Dictionary<string, object>()
                {
                    { "PropertyName" ,failure.PropertyName},
                    { "AttemptedValue", failure.AttemptedValue }
                }));
        }

        return result;
    }
}
