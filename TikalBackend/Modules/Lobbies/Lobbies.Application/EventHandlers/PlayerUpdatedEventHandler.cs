using Lobbies.Application.Mappers;
using Lobbies.Contracts.Notifications;
using Lobbies.Domain.Events;

namespace Lobbies.Application.EventHandlers;

public static class PlayerUpdatedEventHandler
{
    public static PlayerUpdatedNotification Handle(PlayerUpdatedDomainEvent notification, CancellationToken cancellationToken)
    {
        var playerModel = PlayerMapper.PlayerEventDataToLobbyPlayerModel(notification.Player);

        var playerUpdatedNotification = new PlayerUpdatedNotification(playerModel, notification.LobbyId);

        return playerUpdatedNotification;
    }
}