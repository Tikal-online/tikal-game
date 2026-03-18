using System.Diagnostics;
using Identity.WebHost.Pipelines;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Identity.WebHost.ExceptionHandlers;

internal sealed class ValidationExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken
    )
    {
        if (exception is not ValidationException validationException)
        {
            return false;
        }

        httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;

        var errorDictionary =
            validationException.Errors.ToDictionary(error => error.PropertyName, error => error.ErrorMessages);

        var problemDetails = new ProblemDetails
        {
            Status = StatusCodes.Status400BadRequest,
            Title = "One or more validation errors occurred.",
            Extensions = new Dictionary<string, object?>
            {
                { "errors", errorDictionary },
                { "traceId", Activity.Current?.Id ?? httpContext.TraceIdentifier }
            }
        };

        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

        return true;
    }
}