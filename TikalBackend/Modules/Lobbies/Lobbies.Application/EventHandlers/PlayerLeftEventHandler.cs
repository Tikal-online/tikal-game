using Lobbies.Application.Mappers;
using Lobbies.Contracts.Notifications;
using Lobbies.Domain.Events;

namespace Lobbies.Application.EventHandlers;

public static class PlayerLeftEventHandler
{
    public static PlayerLeftNotification Handle(PlayerLeftEvent notification, CancellationToken cancellationToken)
    {
        var playerModel = PlayerMapper.PlayerToLobbyPlayerModel(notification.Player);

        var playerLeftNotification = new PlayerLeftNotification(playerModel, notification.Player.LobbyId);

        return playerLeftNotification;
    }
}