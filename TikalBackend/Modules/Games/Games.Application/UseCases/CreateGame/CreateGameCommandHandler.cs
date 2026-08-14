using Games.Application.DataAccess;
using Games.Contracts.Commands;
using Games.Domain.Entities;
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
        var game = new Game
        {
            LobbyId = 1,
            Players =
            [
                new Player
                {
                    UserId = "testing",
                    Colour = Colour.Red,
                    Points = 10
                }
            ],
            TileMap = new TileMap
            {
                {
                    new HexCoordinate(0, 0),
                    new EmptyTile { Costs = new TravelCosts(northeast: 1) }
                },
                {
                    new HexCoordinate(1, -1),
                    new EmptyTile { Costs = new TravelCosts(southwest: 1) }
                }
            }
        };

        gameRepository.Create(game);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new Success();
    }
}