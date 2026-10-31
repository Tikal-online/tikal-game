using Lobbies.Application.Mappers;
using Lobbies.Contracts.Notifications;
using Lobbies.Domain.Events;

namespace Lobbies.Application.EventHandlers;

public static class PlayerLeftDomainEventHandler
{
    public static PlayerLeftNotification Handle(PlayerLeftDomainEvent notification, CancellationToken cancellationToken)
    {
        var playerModel = PlayerMapper.PlayerEventDataToLobbyPlayerModel(notification.Player);

        var playerLeftNotification = new PlayerLeftNotification(playerModel, notification.LobbyId);

        return playerLeftNotification;
    }
}