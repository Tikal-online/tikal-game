using MediatR;

namespace Shared.Contracts.Messaging;

public interface Query<out TResponse> : IRequest<TResponse>
{
}