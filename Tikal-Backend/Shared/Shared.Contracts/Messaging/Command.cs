using MediatR;

namespace Shared.Contracts.Messaging;

public interface Command<out TResponse> : IRequest<TResponse>
{
}