using Games.Application.DataAccess;
using Games.Application.Mappers;
using Games.Domain.Entities;
using Games.Domain.Types;
using Lobbies.Contracts.Notifications;
using MediatR;

namespace Games.Application.UseCases.CreateGame;

internal sealed class LobbyStartedNotificationHandler : INotificationHandler<LobbyStartedNotification>
{
    private readonly GameRepository gameRepository;

    private readonly UnitOfWork unitOfWork;

    public LobbyStartedNotificationHandler(GameRepository gameRepository, UnitOfWork unitOfWork)
    {
        this.gameRepository = gameRepository;
        this.unitOfWork = unitOfWork;
    }

    public Task Handle(LobbyStartedNotification notification, CancellationToken cancellationToken)
    {
        var players = PlayerMapper.DictionaryToPlayers(notification.Players);

        var game = new Game
        {
            LobbyId = notification.LobbyId,
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
                    Costs = new TravelCosts(NorthEast: 1, Northwest: 1),
                    Coordinate = new HexCoordinate(1, 0)
                }
            ]
        };

        gameRepository.Create(game);

        return unitOfWork.SaveChangesAsync(cancellationToken);
    }
}