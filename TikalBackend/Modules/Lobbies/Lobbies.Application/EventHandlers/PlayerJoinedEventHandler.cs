using Lobbies.Application.Mappers;
using Lobbies.Contracts.Notifications;
using Lobbies.Domain.Events;
using MediatR;
using Shared.Application.Contexts;

namespace Lobbies.Application.EventHandlers;

internal sealed class PlayerJoinedEventHandler : INotificationHandler<PlayerJoinedEvent>
{
    private readonly IPublisher publisher;

    private readonly AccountContext accountContext;

    public PlayerJoinedEventHandler(IPublisher publisher, AccountContext accountContext)
    {
        this.publisher = publisher;
        this.accountContext = accountContext;
    }

    public Task Handle(PlayerJoinedEvent notification, CancellationToken cancellationToken)
    {
        var playerModel = PlayerMapper.PlayerToLobbyPlayerModel(notification.Player, accountContext.Account);

        var playerJoinedNotification = new PlayerJoinedNotification(playerModel, notification.Player.LobbyId);

        return publisher.Publish(playerJoinedNotification, cancellationToken);
    }
}