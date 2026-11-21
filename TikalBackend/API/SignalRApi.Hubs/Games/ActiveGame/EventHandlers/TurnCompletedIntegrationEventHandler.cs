using Games.Contracts.Events;
using Microsoft.AspNetCore.SignalR;
using SignalRApi.Hubs.Games.Mappers;

namespace SignalRApi.Hubs.Games.ActiveGame.EventHandlers;

public sealed class TurnCompletedIntegrationEventHandler
{
    private readonly IHubContext<ActiveGameHub, ActiveGameClient> hubContext;

    public TurnCompletedIntegrationEventHandler(IHubContext<ActiveGameHub, ActiveGameClient> hubContext)
    {
        this.hubContext = hubContext;
    }

    public Task Handle(TurnCompletedIntegrationEvent integrationEvent)
    {
        var gamePlayerDto = GamePlayerModelMapper.GamePlayerModelToGamePlayerDto(integrationEvent.NewPlayer);

        return hubContext.Clients.Group($"{integrationEvent.GameId}").TurnCompleted(gamePlayerDto);
    }
}