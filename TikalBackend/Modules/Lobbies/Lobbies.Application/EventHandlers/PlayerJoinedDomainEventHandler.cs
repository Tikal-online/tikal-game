using Lobbies.Application.Mappers;
using Lobbies.Contracts.Events;
using Lobbies.Domain.Events;

namespace Lobbies.Application.EventHandlers;

public static class PlayerJoinedDomainEventHandler
{
    public static PlayerJoinedIntegrationEvent Handle(PlayerJoinedDomainEvent notification, CancellationToken cancellationToken)
    {
        var playerModel = PlayerMapper.PlayerEventDataToLobbyPlayerModel(notification.Player);

        var playerJoinedNotification = new PlayerJoinedIntegrationEvent(playerModel, notification.LobbyId);

        return playerJoinedNotification;
    }
}