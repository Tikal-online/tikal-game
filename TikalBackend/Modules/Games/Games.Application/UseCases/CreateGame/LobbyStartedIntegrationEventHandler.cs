using Games.Application.DataAccess;
using Games.Application.Mappers;
using Games.Domain.Entities;
using Games.Domain.Types;
using Lobbies.Contracts.Events;

namespace Games.Application.UseCases.CreateGame;

public sealed class LobbyStartedIntegrationEventHandler
{
    private readonly GameRepository gameRepository;

    public LobbyStartedIntegrationEventHandler(GameRepository gameRepository)
    {
        this.gameRepository = gameRepository;
    }

    public void Handle(LobbyStartedIntegrationEvent notification, CancellationToken cancellationToken)
    {
        var players = PlayerMapper.LobbyPlayerModelsToPlayers(notification.Lobby.Players);

        var game = new Game
        {
            LobbyId = notification.Lobby.Id,
            Players = players,
            Tiles =
            [
                new EmptyTile
                {
                    Costs = new TravelCosts(1, 1, 1),
                    Coordinate = new HexCoordinate(0, 0)
                },
                new TempleTile
                {
                    TempleLevel = 1,
                    Costs = new TravelCosts(1, 1, South: 1),
                    Coordinate = new HexCoordinate(0, -1)
                },
                new EmptyTile
                {
                    Costs = new TravelCosts(1, 1, 1),
                    Coordinate = new HexCoordinate(1, -1)
                },
                new TempleTile
                {
                    TempleLevel = 1,
                    Costs = new TravelCosts(NorthEast: 1, NorthWest: 1),
                    Coordinate = new HexCoordinate(1, 0)
                }
            ]
        };

        gameRepository.Create(game);
    }
}