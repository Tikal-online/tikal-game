using Lobbies.Application.Mappers;
using Lobbies.Contracts.Events;
using Lobbies.Domain.Events;

namespace Lobbies.Application.EventHandlers;

public static class LobbyStartedDomainEventHandler
{
    public static LobbyStartedIntegrationEvent Handle(LobbyStartedDomainEvent notification, CancellationToken cancellationToken)
    {
        var lobbyModel = LobbyMapper.LobbyEventDataToLobbyModel(notification.Lobby);

        var lobbyStartedNotification = new LobbyStartedIntegrationEvent(lobbyModel);

        return lobbyStartedNotification;
    }
}