using ClinicApp.Application.Converters;
using ClinicApp.Domain.Repositories;
using ClinicApp.Domain.SessionAgg;
using ErrorOr;
using FluentValidation;
using MediatR;

namespace ClinicApp.Application.Queries.Sessions.SessionHistory;
public record GetSessionHistoryQuery(string Id) : 
    IRequest<ErrorOr<IReadOnlyCollection<SessionState>>>;

public sealed class GetSessionHistoryQueryHandler :
    IRequestHandler<GetSessionHistoryQuery, ErrorOr<IReadOnlyCollection<SessionState>>>
{
    ISessionRepository _repo;
    IValidator<GetSessionHistoryQuery> _validator;
    public GetSessionHistoryQueryHandler(ISessionRepository repo, IValidator<GetSessionHistoryQuery> validator)
    {
        _repo = repo;
        _validator = validator;
    }

    public async Task<ErrorOr<IReadOnlyCollection<SessionState>>> Handle(GetSessionHistoryQuery request, CancellationToken cancellationToken)
    {
        var validationResult = _validator.Validate(request);
        if (!validationResult.IsValid)
        {
            return validationResult.Errors.FromValidationToErrors();
        }

        IReadOnlyCollection<SessionState> result = await _repo.GetSessionHistory(Guid.Parse(request.Id));
        return result.ToErrorOr();
    }
}


public class GetSessionHistoryQueryValidator
    : AbstractValidator<GetSessionHistoryQuery>
{
    public GetSessionHistoryQueryValidator()
    {
        RuleFor(q => q.Id)
            .NotEmpty()
            .WithErrorCode("Application.Validation")
            .WithMessage("Id is empty")
            .Must((q,s) => Guid.TryParse(q.Id,out _));
    }
}