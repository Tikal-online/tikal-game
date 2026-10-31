using Lobbies.Contracts.Notifications;
using Microsoft.AspNetCore.SignalR;
using SignalRApi.Hubs.Lobbies.Mappers;

namespace SignalRApi.Hubs.Lobbies.ActiveLobby.NotificationHandlers;

public sealed class PlayerUpdatedNotificationHandler
{
    private readonly IHubContext<ActiveLobbyHub, ActiveLobbyClient> hubContext;

    public PlayerUpdatedNotificationHandler(IHubContext<ActiveLobbyHub, ActiveLobbyClient> hubContext)
    {
        this.hubContext = hubContext;
    }

    public Task Handle(PlayerUpdatedNotification notification, CancellationToken cancellationToken)
    {
        var lobbyPlayerDto = LobbyPlayerModelMapper.LobbyPlayerModelToLobbyPlayerDto(notification.LobbyPlayerModel);

        return hubContext.Clients.Group($"{notification.LobbyId}").PlayerUpdated(lobbyPlayerDto);
    }
}