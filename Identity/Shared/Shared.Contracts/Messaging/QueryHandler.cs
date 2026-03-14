using MediatR;

namespace Shared.Contracts.Messaging;

public interface QueryHandler<in TQuery, TResponse> : IRequestHandler<TQuery, TResponse>
    where TQuery : Query<TResponse>
{
}