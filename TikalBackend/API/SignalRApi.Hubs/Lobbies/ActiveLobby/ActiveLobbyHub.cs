using Lobbies.Contracts.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Wolverine;

namespace SignalRApi.Hubs.Lobbies.ActiveLobby;

[Authorize]
public sealed class ActiveLobbyHub : Hub<ActiveLobbyClient>
{
    private readonly IMessageBus bus;

    public ActiveLobbyHub(IMessageBus bus)
    {
        this.bus = bus;
    }

    public override async Task OnConnectedAsync()
    {
        var lobbyId = await bus.InvokeAsync<long?>(new GetLobbyIdForAuthenticatedPlayerQuery());

        if (lobbyId is null)
        {
            throw new HubException("Player is not in a lobby");
        }

        await Groups.AddToGroupAsync(Context.ConnectionId, $"{lobbyId}");

        await base.OnConnectedAsync();

        await Clients.Caller.InitializationComplete();
    }
}