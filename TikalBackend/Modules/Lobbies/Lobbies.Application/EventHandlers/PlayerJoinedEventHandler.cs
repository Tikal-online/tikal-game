using Lobbies.Application.Mappers;
using Lobbies.Contracts.Notifications;
using Lobbies.Domain.Events;

namespace Lobbies.Application.EventHandlers;

public sealed class PlayerJoinedEventHandler
{
    public static PlayerJoinedNotification Handle(PlayerJoinedEvent notification, CancellationToken cancellationToken)
    {
        var playerModel = PlayerMapper.PlayerEventDataToLobbyPlayerModel(notification.Player);

        var playerJoinedNotification = new PlayerJoinedNotification(playerModel, notification.LobbyId);

        return playerJoinedNotification;
    }
}