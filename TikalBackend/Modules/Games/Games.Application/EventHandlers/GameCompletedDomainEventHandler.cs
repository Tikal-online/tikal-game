using Games.Application.Mappers;
using Games.Contracts.Events;
using Games.Domain.Events;

namespace Games.Application.EventHandlers;

public static class GameCompletedDomainEventHandler
{
    public static GameCompletedIntegrationEvent Handle(GameCompletedDomainEvent domainEvent, CancellationToken cancellationToken)
    {
        var gameModel = GameMapper.GameEventDataToGameModel(domainEvent.Game);

        var gameCompletedIntegrationEvent = new GameCompletedIntegrationEvent(gameModel);

        return gameCompletedIntegrationEvent;
    }
}