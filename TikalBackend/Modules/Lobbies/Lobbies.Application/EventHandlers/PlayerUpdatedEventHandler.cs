using Accounts.Contracts.Queries;
using Lobbies.Application.Mappers;
using Lobbies.Contracts.Notifications;
using Lobbies.Domain.Events;
using MediatR;

namespace Lobbies.Application.EventHandlers;

internal sealed class PlayerUpdatedEventHandler : INotificationHandler<PlayerUpdatedEvent>
{
    private readonly IPublisher publisher;

    private readonly ISender sender;

    public PlayerUpdatedEventHandler(ISender sender, IPublisher publisher)
    {
        this.sender = sender;
        this.publisher = publisher;
    }

    public async Task Handle(PlayerUpdatedEvent notification, CancellationToken cancellationToken)
    {
        var playerAccount = await sender.Send(new GetAccountQuery(notification.Player.UserId), cancellationToken);

        var playerModel = PlayerMapper.PlayerToLobbyPlayerModel(notification.Player, playerAccount!);

        var playerUpdatedNotification = new PlayerUpdatedNotification(playerModel, notification.Player.LobbyId);

        await publisher.Publish(playerUpdatedNotification, cancellationToken);
    }
}