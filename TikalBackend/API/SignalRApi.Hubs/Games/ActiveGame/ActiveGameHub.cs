using Games.Contracts.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Shared.Api;
using Wolverine;

namespace SignalRApi.Hubs.Games.ActiveGame;

[Authorize]
public sealed class ActiveGameHub : Hub<ActiveGameClient>
{
    private readonly AccountContext accountContext;

    private readonly IMessageBus bus;

    public ActiveGameHub(IMessageBus bus, AccountContext accountContext)
    {
        this.bus = bus;
        this.accountContext = accountContext;
    }

    public override async Task OnConnectedAsync()
    {
        var gameId = await bus.InvokeAsync<long?>(new GetGameIdForPlayerQuery(accountContext.Account.UserId));

        if (gameId is null)
        {
            throw new HubException("Player is not in a game");
        }

        await Groups.AddToGroupAsync(Context.ConnectionId, $"{gameId}");

        await base.OnConnectedAsync();

        await Clients.Caller.InitializationComplete();
    }
}