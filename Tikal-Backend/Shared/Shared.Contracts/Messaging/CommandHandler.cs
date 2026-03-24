using MediatR;

namespace Shared.Contracts.Messaging;

public interface CommandHandler<in TCommand, TResponse> : IRequestHandler<TCommand, TResponse>
    where TCommand : Command<TResponse>
{
}