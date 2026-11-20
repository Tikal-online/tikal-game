using Games.Contracts.Events;
using Microsoft.AspNetCore.SignalR;

namespace SignalRApi.Hubs.Games.ActiveGame.EventHandlers;

public sealed class GameCompletedIntegrationEventHandler
{
    private readonly IHubContext<ActiveGameHub, ActiveGameClient> hubContext;

    public GameCompletedIntegrationEventHandler(IHubContext<ActiveGameHub, ActiveGameClient> hubContext)
    {
        this.hubContext = hubContext;
    }

    public Task Handle(GameCompletedIntegrationEvent integrationEvent)
    {
        return hubContext.Clients.Groups($"{integrationEvent.Game.Id}").GameCompleted();
    }
}