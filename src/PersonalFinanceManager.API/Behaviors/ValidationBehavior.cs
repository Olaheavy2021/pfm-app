using System.Net;
using FluentValidation;
using FluentValidation.Results;
using MediatR;

namespace PersonalFinanceManager.API.Behaviors;

public sealed class ValidationBehavior<TRequest, TResponse>(
    IEnumerable<IValidator<TRequest>> validators
) : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    where TResponse : IResult
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken
    )
    {
        if (!validators.Any())
            return await next();

        var context = new ValidationContext<TRequest>(request);

        var validationErrors = validators
            .Select(x => x.Validate(context))
            .SelectMany(x => x.Errors)
            .Where(x => x != null)
            .ToArray();

        var statusCode = GetStatusCode(validationErrors);

        var errorsDictionary = validationErrors
            .GroupBy(
                x => x.PropertyName,
                x => x.ErrorMessage,
                (propertyName, errorMessages) =>
                    new { Key = propertyName, Values = errorMessages.Distinct().ToArray() }
            )
            .ToDictionary(x => x.Key, x => x.Values);

        return errorsDictionary.Any()
            ? (TResponse)Results.ValidationProblem(errorsDictionary, statusCode: (int)statusCode)
            : await next();
    }

    private static HttpStatusCode GetStatusCode(IEnumerable<ValidationFailure> validationErrors)
    {
        var codes = validationErrors
            .Select(x => x.CustomState)
            .Where(x => x is HttpStatusCode)
            .OfType<HttpStatusCode>()
            .Distinct()
            .ToArray();

        return codes.Length == 1 ? codes.First() : HttpStatusCode.BadRequest;
    }
}
