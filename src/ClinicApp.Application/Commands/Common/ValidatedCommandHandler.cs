using ClinicApp.Application.Converters;
using ErrorOr;
using FluentValidation;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicApp.Application.Commands.Common;
public abstract class ValidatedCommandHandler<TCommand, TResponse> : IRequestHandler<TCommand, ErrorOr<TResponse>>
    where TCommand : IRequest<ErrorOr<TResponse>>
    where TResponse : class
{
    private readonly IValidator<TCommand> _validator;

    public ValidatedCommandHandler(IValidator<TCommand> validator)
    {
        _validator = validator;
    }
    /// <summary>
    /// The Method to run after the command has been validated
    /// </summary>
    /// <returns></returns>
    public abstract Task<ErrorOr<TResponse>> GetResponse(TCommand command, CancellationToken cancellationToken);
    

    public async Task<ErrorOr<TResponse>> Handle(TCommand request, CancellationToken cancellationToken)
    {
        var validate = _validator.Validate(request);

        if (!validate.IsValid)
        {
            return validate.Errors.FromValidationToErrors();
        }

        var result = await GetResponse(request,cancellationToken);
        return result;
    }
}

