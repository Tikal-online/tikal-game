using Lobbies.Application.Mappers;
using Lobbies.Contracts.Notifications;
using Lobbies.Domain.Events;

namespace Lobbies.Application.EventHandlers;

public static class PlayerJoinedDomainEventHandler
{
    public static PlayerJoinedNotification Handle(PlayerJoinedDomainEvent notification, CancellationToken cancellationToken)
    {
        var playerModel = PlayerMapper.PlayerEventDataToLobbyPlayerModel(notification.Player);

        var playerJoinedNotification = new PlayerJoinedNotification(playerModel, notification.LobbyId);

        return playerJoinedNotification;
    }
}