using Games.Application.DataAccess;
using Games.Contracts.Commands;
using Games.Domain.Entities;
using Games.Domain.Enums;
using Games.Domain.Types;
using OneOf.Types;
using Shared.Contracts.Messaging;
using Shared.Domain.Enums;

namespace Games.Application.UseCases.CreateGame;

internal sealed class CreateGameCommandHandler
    : CommandHandler<CreateGameCommand, Success>
{
    private readonly GameRepository gameRepository;

    private readonly UnitOfWork unitOfWork;

    public CreateGameCommandHandler(GameRepository gameRepository, UnitOfWork unitOfWork)
    {
        this.gameRepository = gameRepository;
        this.unitOfWork = unitOfWork;
    }

    public async Task<Success> Handle(CreateGameCommand request, CancellationToken cancellationToken)
    {
        var player = new Player
        {
            UserId = "userId",
            Colour = Colour.Red,
            Points = 0
        };

        var game = new Game
        {
            LobbyId = 1,
            Players =
            [
                player
            ],
            TileMap = new TileMap
            {
                Tiles =
                [
                    new EmptyTile
                    {
                        Costs = new TravelCosts(SouthEast: 1, Northwest: 10),
                        Coordinate = new HexCoordinate(4, 7),
                        TroopAssignments =
                        [
                            new TroopAssignment
                            {
                                TroopType = TroopType.Leader,
                                Count = 1,
                                Player = player
                            }
                        ]
                    },
                    new TempleTile
                    {
                        Costs = new TravelCosts(SouthEast: 1, South: 10),
                        Coordinate = new HexCoordinate(4, 8)
                    }
                ]
            }
        };

        gameRepository.Create(game);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new Success();
    }
}