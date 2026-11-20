using Games.Contracts.Events;
using Microsoft.AspNetCore.SignalR;
using SignalRApi.Hubs.Games.Mappers;

namespace SignalRApi.Hubs.Games.ActiveGame.EventHandlers;

public sealed class GamePlayerLeftIntegrationEventHandler
{
    private readonly IHubContext<ActiveGameHub, ActiveGameClient> hubContext;

    public GamePlayerLeftIntegrationEventHandler(IHubContext<ActiveGameHub, ActiveGameClient> hubContext)
    {
        this.hubContext = hubContext;
    }

    public Task Handle(PlayerLeftIntegrationEvent integrationEvent)
    {
        var gamePlayerDto = GamePlayerModelMapper.GamePlayerModelToGamePlayerDto(integrationEvent.GamePlayerModel);

        return hubContext.Clients.Group($"{integrationEvent.GameId}").PlayerLeft(gamePlayerDto);
    }
}