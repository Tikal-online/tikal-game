using Lobbies.Contracts.Notifications;
using Lobbies.Domain.Events;

namespace Lobbies.Application.EventHandlers;

public sealed class LobbyStartedEventHandler
{
    public static LobbyStartedNotification Handle(LobbyStartedEvent notification, CancellationToken cancellationToken)
    {
        // TODO: pass players
        var lobbyStartedNotification = new LobbyStartedNotification(notification.lobby.Id);

        return lobbyStartedNotification;
    }
}