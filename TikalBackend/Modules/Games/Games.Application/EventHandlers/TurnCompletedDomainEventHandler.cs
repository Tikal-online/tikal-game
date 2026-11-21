using Games.Application.Mappers;
using Games.Contracts.Events;
using Games.Domain.Events;

namespace Games.Application.EventHandlers;

public static class TurnCompletedDomainEventHandler
{
    public static TurnCompletedIntegrationEvent Handle(TurnCompleteDomainEvent domainEvent)
    {
        var playerModel = PlayerMapper.PlayerEventDataToGamePlayerModel(domainEvent.NewPlayer);

        var turnCompletedIntegrationEvent = new TurnCompletedIntegrationEvent(playerModel, domainEvent.GameId);

        return turnCompletedIntegrationEvent;
    }
}