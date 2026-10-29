using Lobbies.Application.Mappers;
using Lobbies.Contracts.Notifications;
using Lobbies.Domain.Events;

namespace Lobbies.Application.EventHandlers;

public sealed class PlayerUpdatedEventHandler
{
    public static PlayerUpdatedNotification Handle(PlayerUpdatedEvent notification, CancellationToken cancellationToken)
    {
        var playerModel = PlayerMapper.PlayerToLobbyPlayerModel(notification.Player);

        var playerUpdatedNotification = new PlayerUpdatedNotification(playerModel, notification.Player.LobbyId);

        return playerUpdatedNotification;
    }
}