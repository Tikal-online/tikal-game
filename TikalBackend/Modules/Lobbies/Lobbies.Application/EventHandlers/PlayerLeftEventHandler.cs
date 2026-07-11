using Lobbies.Application.Mappers;
using Lobbies.Contracts.Notifications;
using Lobbies.Domain.Events;
using MediatR;
using Shared.Application.Contexts;

namespace Lobbies.Application.EventHandlers;

internal sealed class PlayerLeftEventHandler : INotificationHandler<PlayerLeftEvent>
{
    private readonly IPublisher publisher;

    private readonly AccountContext accountContext;

    public PlayerLeftEventHandler(IPublisher publisher, AccountContext accountContext)
    {
        this.publisher = publisher;
        this.accountContext = accountContext;
    }

    public Task Handle(PlayerLeftEvent notification, CancellationToken cancellationToken)
    {
        var playerModel = PlayerMapper.PlayerToLobbyPlayerModel(notification.Player, accountContext.Account);

        var playerLeftNotification = new PlayerLeftNotification(playerModel, notification.Player.LobbyId);

        return publisher.Publish(playerLeftNotification, cancellationToken);
    }
}