using Lobbies.Application.Mappers;
using Lobbies.Contracts.Notifications;
using Lobbies.Domain.Events;

namespace Lobbies.Application.EventHandlers;

public static class LobbyStartedEventHandler
{
    public static LobbyStartedNotification Handle(LobbyStartedDomainEvent notification, CancellationToken cancellationToken)
    {
        var lobbyModel = LobbyMapper.LobbyEventDataToLobbyModel(notification.Lobby);

        var lobbyStartedNotification = new LobbyStartedNotification(lobbyModel);

        return lobbyStartedNotification;
    }
}