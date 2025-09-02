using ClinicApp.Domain.Common.Interfaces;
using ClinicApp.Domain.Common.ValueObjects;
using ClinicApp.Domain.SessionAgg;
using FluentValidation;

namespace ClinicApp.Presentation.Validation;

public class AddSessionRequestValidator : AbstractValidator<AddSessionRequest>
{
    private readonly IClock _clock;

    public AddSessionRequestValidator(IClock clock)
    {
        _clock = clock;
        RuleFor(s => s.StartTime)
            .Must(d => d > _clock.UtcNow)
            .WithMessage($"Can't Book in the Past")
            .WithErrorCode("API.Validation.Session");

        RuleFor(s => s.EndTime)
            .Must(d => d > _clock.UtcNow)
            .WithMessage($"Can't Book in the Past")
            .WithErrorCode("API.Validation.Session");

        RuleFor(s => s)
            .Must(s => s.StartTime < s.EndTime)
            .WithMessage("StartTime is after EndTime")
            .WithErrorCode("API.Validation.Session");
    }

}
