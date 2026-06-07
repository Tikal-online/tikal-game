using Lobbies.Contracts.Commands;
using Lobbies.Contracts.Notifications;
using MediatR;
using OneOf.Types;
using Shared.Application.Contexts;
using Shared.Contracts.Messaging;

namespace Lobbies.Application.UseCases.SendGlobalChatMessage;

internal sealed class SendGlobalChatMessageCommandHandler : CommandHandler<SendGlobalChatMessageCommand, Success>
{
    private readonly AccountContext accountContext;

    private readonly ISender sender;

    public SendGlobalChatMessageCommandHandler(AccountContext accountContext, ISender sender)
    {
        this.accountContext = accountContext;
        this.sender = sender;
    }

    public async Task<Success> Handle(SendGlobalChatMessageCommand request, CancellationToken cancellationToken)
    {
        var message = $"[{accountContext.Account.Name}]: {request.messageContent}";

        var notification = new GlobalChatMessageSentNotification(message);

        await sender.Send(notification, cancellationToken);

        return new Success();
    }
}