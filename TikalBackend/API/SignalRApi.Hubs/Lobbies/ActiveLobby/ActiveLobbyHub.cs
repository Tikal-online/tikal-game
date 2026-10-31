using Lobbies.Contracts.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Shared.Api;
using Wolverine;

namespace SignalRApi.Hubs.Lobbies.ActiveLobby;

[Authorize]
public sealed class ActiveLobbyHub : Hub<ActiveLobbyClient>
{
    private readonly AccountContext accountContext;

    private readonly IMessageBus bus;

    public ActiveLobbyHub(IMessageBus bus, AccountContext accountContext)
    {
        this.bus = bus;
        this.accountContext = accountContext;
    }

    public override async Task OnConnectedAsync()
    {
        var lobbyId = await bus.InvokeAsync<long?>(new GetLobbyIdForPlayerQuery(accountContext.Account.UserId));

        if (lobbyId is null)
        {
            throw new HubException("Player is not in a lobby");
        }

        await Groups.AddToGroupAsync(Context.ConnectionId, $"{lobbyId}");

        await base.OnConnectedAsync();

        await Clients.Caller.InitializationComplete();
    }
}