using Lobbies.Contracts.Commands;
using Lobbies.Contracts.Models;
using Lobbies.Contracts.Notifications;
using MediatR;
using OneOf.Types;
using Shared.Application.Contexts;
using Shared.Contracts.Messaging;

namespace Lobbies.Application.UseCases.SendGlobalChatMessage;

internal sealed class SendGlobalChatMessageCommandHandler : CommandHandler<SendGlobalChatMessageCommand, Success>
{
    private readonly AccountContext accountContext;

    private readonly IPublisher publisher;

    public SendGlobalChatMessageCommandHandler(AccountContext accountContext, IPublisher publisher)
    {
        this.accountContext = accountContext;
        this.publisher = publisher;
    }

    public async Task<Success> Handle(SendGlobalChatMessageCommand request, CancellationToken cancellationToken)
    {
        var message = new ChatMessageModel
        {
            UserId = accountContext.Account.UserId,
            Username = accountContext.Account.Name,
            Content = request.messageContent
        };

        var notification = new GlobalChatMessageSentNotification(message);

        await publisher.Publish(notification, cancellationToken);

        return new Success();
    }
}