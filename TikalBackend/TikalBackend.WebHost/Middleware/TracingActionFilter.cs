using System.Diagnostics;
using Microsoft.AspNetCore.Mvc.Filters;
using TikalBackend.WebHost.Telemetry;

namespace TikalBackend.WebHost.Middleware;

internal sealed class TracingActionFilter : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var actionName = $"{context.Controller.GetType().Name}/{context.ActionDescriptor.DisplayName}";
        using var activity = ActivitySources.ControllerSource.StartActivity(actionName, ActivityKind.Server);

        var result = await next();

        if (result.Exception is not null && !result.ExceptionHandled)
        {
            activity?.SetStatus(ActivityStatusCode.Error, result.Exception.Message);
            activity?.AddException(result.Exception);
        }
    }
}