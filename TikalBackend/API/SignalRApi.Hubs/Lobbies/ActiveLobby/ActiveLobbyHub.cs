using Lobbies.Contracts.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace SignalRApi.Hubs.Lobbies.ActiveLobby;

[Authorize]
public sealed class ActiveLobbyHub : Hub<ActiveLobbyClient>
{
    private readonly ISender sender;

    public ActiveLobbyHub(ISender sender)
    {
        this.sender = sender;
    }

    public override async Task OnConnectedAsync()
    {
        var lobbyId = await sender.Send(new GetLobbyIdForAuthenticatedPlayerQuery());

        if (lobbyId is null)
        {
            throw new HubException("Player is not in a lobby");
        }

        await Groups.AddToGroupAsync(Context.ConnectionId, $"{lobbyId}");

        await base.OnConnectedAsync();
    }
}