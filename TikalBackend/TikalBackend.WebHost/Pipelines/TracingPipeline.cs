using System.Diagnostics;
using MediatR;
using TikalBackend.WebHost.Telemetry;

namespace TikalBackend.WebHost.Pipelines;

internal sealed class TracingPipeline<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken
    )
    {
        var requestName = typeof(TRequest).Name;
        using var activity = ActivitySources.MediatRSource.StartActivity($"MediatR {requestName}", ActivityKind.Server);

        try
        {
            return await next(cancellationToken);
        }
        catch (Exception ex)
        {
            activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
            activity?.AddException(ex);
            throw;
        }
    }
}