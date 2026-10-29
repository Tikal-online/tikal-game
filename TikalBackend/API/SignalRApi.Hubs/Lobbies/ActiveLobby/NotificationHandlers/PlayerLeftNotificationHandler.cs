using Lobbies.Contracts.Notifications;
using Microsoft.AspNetCore.SignalR;
using SignalRApi.Hubs.Lobbies.Mappers;

namespace SignalRApi.Hubs.Lobbies.ActiveLobby.NotificationHandlers;

public sealed class PlayerLeftNotificationHandler
{
    private readonly IHubContext<ActiveLobbyHub, ActiveLobbyClient> hubContext;

    public PlayerLeftNotificationHandler(IHubContext<ActiveLobbyHub, ActiveLobbyClient> hubContext)
    {
        this.hubContext = hubContext;
    }

    public Task Handle(PlayerLeftNotification notification, CancellationToken cancellationToken)
    {
        var lobbyPlayerDto = LobbyPlayerModelMapper.LobbyPlayerModelToLobbyPlayerDto(notification.LobbyPlayerModel);

        return hubContext.Clients.Group($"{notification.LobbyId}").PlayerLeft(lobbyPlayerDto);
    }
}