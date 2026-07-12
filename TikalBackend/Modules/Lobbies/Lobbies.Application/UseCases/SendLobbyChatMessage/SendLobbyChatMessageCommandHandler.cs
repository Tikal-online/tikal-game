using Lobbies.Application.DataAccess;
using Lobbies.Contracts.Commands;
using Lobbies.Contracts.Errors;
using Lobbies.Contracts.Models;
using Lobbies.Contracts.Notifications;
using MediatR;
using OneOf;
using OneOf.Types;
using Shared.Application.Contexts;
using Shared.Contracts.Messaging;

namespace Lobbies.Application.UseCases.SendLobbyChatMessage;

internal sealed class SendLobbyChatMessageCommandHandler
    : CommandHandler<SendLobbyChatMessageCommand, OneOf<Success, PlayerNotInALobby>>
{
    private readonly LobbyQueryContext lobbyQueryContext;

    private readonly IPublisher publisher;

    private readonly AccountContext accountContext;

    public SendLobbyChatMessageCommandHandler(
        LobbyQueryContext lobbyQueryContext,
        IPublisher publisher,
        AccountContext accountContext
    )
    {
        this.lobbyQueryContext = lobbyQueryContext;
        this.publisher = publisher;
        this.accountContext = accountContext;
    }

    public async Task<OneOf<Success, PlayerNotInALobby>> Handle(
        SendLobbyChatMessageCommand request,
        CancellationToken cancellationToken
    )
    {
        var lobbyId = await lobbyQueryContext.GetIdByUserIdAsync(accountContext.Account.UserId);

        if (lobbyId is null)
        {
            return new PlayerNotInALobby();
        }

        var message = new ChatMessageModel
        {
            UserId = accountContext.Account.UserId,
            Username = accountContext.Account.Name,
            Content = request.MessageContent
        };

        var notification = new LobbyChatMessageSentNotification(message, lobbyId.Value);

        await publisher.Publish(notification, cancellationToken);

        return new Success();
    }
}