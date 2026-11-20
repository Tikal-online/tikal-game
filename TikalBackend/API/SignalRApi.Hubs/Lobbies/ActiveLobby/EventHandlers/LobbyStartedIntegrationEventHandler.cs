using Lobbies.Contracts.Events;
using Microsoft.AspNetCore.SignalR;

namespace SignalRApi.Hubs.Lobbies.ActiveLobby.EventHandlers;

public sealed class LobbyStartedIntegrationEventHandler
{
    private readonly IHubContext<ActiveLobbyHub, ActiveLobbyClient> hubContext;

    public LobbyStartedIntegrationEventHandler(IHubContext<ActiveLobbyHub, ActiveLobbyClient> hubContext)
    {
        this.hubContext = hubContext;
    }

    public Task Handle(LobbyStartedIntegrationEvent notification)
    {
        return hubContext.Clients.Group($"{notification.Lobby.Id}").LobbyStarted();
    }
}