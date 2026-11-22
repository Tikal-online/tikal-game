using Games.Application.Mappers;
using Games.Contracts.Events;
using Games.Domain.Events;

namespace Games.Application.EventHandlers;

public static class TilePlacedIntegrationEventHandler
{
    public static TilePlacedIntegrationEvent Handle(TilePlacedDomainEvent domainEvent)
    {
        var tileModel = GameMapper.TileEventDataToTileModel(domainEvent.Tile);

        var tilePlacedIntegrationEvent = new TilePlacedIntegrationEvent(tileModel, domainEvent.GameId);

        return tilePlacedIntegrationEvent;
    }
}