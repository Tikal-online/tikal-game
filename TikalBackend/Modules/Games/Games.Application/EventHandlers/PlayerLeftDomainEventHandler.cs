using Games.Application.Mappers;
using Games.Contracts.Events;
using Games.Domain.Events;

namespace Games.Application.EventHandlers;

public static class PlayerLeftDomainEventHandler
{
    public static PlayerLeftIntegrationEvent Handle(PlayerLeftDomainEvent domainEvent)
    {
        var playerModel = PlayerMapper.PlayerEventDataToGamePlayerModel(domainEvent.Player);

        var playerLeftIntegrationEvent = new PlayerLeftIntegrationEvent(playerModel, domainEvent.LobbyId);

        return playerLeftIntegrationEvent;
    }
}