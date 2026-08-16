using Lobbies.Application.Mappers;
using Lobbies.Contracts.Notifications;
using Lobbies.Domain.Events;
using MediatR;

namespace Lobbies.Application.EventHandlers;

internal sealed class LobbyStartedEventHandler : INotificationHandler<LobbyStartedEvent>
{
    private readonly IPublisher publisher;

    public LobbyStartedEventHandler(IPublisher publisher)
    {
        this.publisher = publisher;
    }

    public Task Handle(LobbyStartedEvent notification, CancellationToken cancellationToken)
    {
        var players = PlayerMapper.PlayersToDictionary(notification.lobby.Players);

        var lobbyStartedNotification = new LobbyStartedNotification(notification.lobby.Id, players);

        return publisher.Publish(lobbyStartedNotification, cancellationToken);
    }
}