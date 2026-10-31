using Lobbies.Application.Mappers;
using Lobbies.Contracts.Events;
using Lobbies.Domain.Events;

namespace Lobbies.Application.EventHandlers;

public static class PlayerLeftDomainEventHandler
{
    public static PlayerLeftIntegrationEvent Handle(PlayerLeftDomainEvent notification, CancellationToken cancellationToken)
    {
        var playerModel = PlayerMapper.PlayerEventDataToLobbyPlayerModel(notification.Player);

        var playerLeftNotification = new PlayerLeftIntegrationEvent(playerModel, notification.LobbyId);

        return playerLeftNotification;
    }
}