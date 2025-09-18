using ClinicApp.Application.Commands.AddSessionsCommands;
using ClinicApp.Domain.Common.Interfaces;
using FluentValidation;

namespace ClinicApp.Application.Validation;

public class AddSessionCommandValidator : AbstractValidator<AddSessionCommand>
{
    private readonly IClock _clock;

    public AddSessionCommandValidator(IClock clock)
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
