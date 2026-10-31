using Lobbies.Application.Mappers;
using Lobbies.Contracts.Events;
using Lobbies.Domain.Events;

namespace Lobbies.Application.EventHandlers;

public static class PlayerUpdatedDomainEventHandler
{
    public static PlayerUpdatedIntegrationEvent Handle(PlayerUpdatedDomainEvent notification, CancellationToken cancellationToken)
    {
        var playerModel = PlayerMapper.PlayerEventDataToLobbyPlayerModel(notification.Player);

        var playerUpdatedNotification = new PlayerUpdatedIntegrationEvent(playerModel, notification.LobbyId);

        return playerUpdatedNotification;
    }
}