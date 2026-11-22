using Games.Contracts.Events;
using Microsoft.AspNetCore.SignalR;
using SignalRApi.Hubs.Games.Mappers;

namespace SignalRApi.Hubs.Games.ActiveGame.EventHandlers;

public sealed class TilePlacedIntegrationEventHandler
{
    private readonly IHubContext<ActiveGameHub, ActiveGameClient> hubContext;

    public TilePlacedIntegrationEventHandler(IHubContext<ActiveGameHub, ActiveGameClient> hubContext)
    {
        this.hubContext = hubContext;
    }

    public Task Handle(TilePlacedIntegrationEvent integrationEvent)
    {
        var tileDto = TileModelMapper.TileModelToTileDto(integrationEvent.Tile);

        return hubContext.Clients.Group($"{integrationEvent.GameId}").TilePlaced(tileDto);
    }
}