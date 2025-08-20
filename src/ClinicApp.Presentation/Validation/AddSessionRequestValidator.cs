using ClinicApp.Domain.Common.Interfaces;
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
            .Must(d => d > _clock.UtcNow);

        RuleFor(s => s.EndTime)
            .Must(d => d > _clock.UtcNow);
    }

}
