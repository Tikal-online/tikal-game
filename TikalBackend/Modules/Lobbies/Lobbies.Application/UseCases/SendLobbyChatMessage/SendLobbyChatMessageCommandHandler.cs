using Lobbies.Application.DataAccess;
using Lobbies.Contracts.Commands;
using Lobbies.Contracts.Errors;
using Lobbies.Contracts.Models;
using Lobbies.Contracts.Notifications;
using OneOf;

namespace Lobbies.Application.UseCases.SendLobbyChatMessage;

public sealed class SendLobbyChatMessageCommandHandler
{
    private readonly LobbyQueryContext lobbyQueryContext;

    public SendLobbyChatMessageCommandHandler(LobbyQueryContext lobbyQueryContext)
    {
        this.lobbyQueryContext = lobbyQueryContext;
    }

    public async Task<OneOf<LobbyChatMessageSentNotification, LobbyNotFound, PlayerNotInGivenLobby>> Handle(
        SendLobbyChatMessageCommand request,
        CancellationToken cancellationToken
    )
    {
        var lobby = await lobbyQueryContext.GetById(request.LobbyId);

        if (lobby is null)
        {
            return new LobbyNotFound(request.LobbyId);
        }

        var player = lobby.GetPlayer(request.UserId);

        if (player is null)
        {
            return new PlayerNotInGivenLobby(request.LobbyId);
        }

        var message = new ChatMessageModel
        {
            UserId = player.UserId,
            Username = player.Name,
            Content = request.MessageContent
        };

        var notification = new LobbyChatMessageSentNotification(message, lobby.Id);

        return notification;
    }
}