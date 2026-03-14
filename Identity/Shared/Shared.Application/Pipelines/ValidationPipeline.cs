using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Shared.Contracts.Errors;

namespace Shared.Application.Pipelines;

public class ValidationException : Exception
{
    public IReadOnlyList<ValidationError> Errors { get; }

    public ValidationException(IReadOnlyList<ValidationError> errors)
    {
        Errors = errors;
    }
}

public class ValidationPipeline<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IEnumerable<IValidator<TRequest>> validators;

    public ValidationPipeline(IEnumerable<IValidator<TRequest>> validators)
    {
        this.validators = validators;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken
    )
    {
        var validationResult = ValidateAsync(request);

        if (validationResult.IsValid)
        {
            return await next(cancellationToken);
        }

        var errors = validationResult.Errors
            .GroupBy(error => error.PropertyName)
            .Select(grouping => new ValidationError(grouping.Key, grouping.Select(g => g.ErrorMessage).ToList()))
            .ToList();

        throw new ValidationException(errors);
    }

    private ValidationResult ValidateAsync(TRequest request)
    {
        if (!validators.Any())
        {
            return new ValidationResult();
        }

        IEnumerable<ValidationFailure> errors = validators
            .Select(validator => validator.Validate(request))
            .SelectMany(result => result.Errors)
            .Where(error => error is not null)
            .Distinct();

        return new ValidationResult(errors);
    }
}