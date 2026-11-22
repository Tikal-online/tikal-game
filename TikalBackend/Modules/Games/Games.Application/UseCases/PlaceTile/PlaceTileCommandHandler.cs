using Games.Application.DataAccess;
using Games.Contracts.Commands;
using Games.Contracts.Errors;
using OneOf;
using OneOf.Types;
using Shared.Contracts.Errors;

namespace Games.Application.UseCases.PlaceTile;

public sealed class PlaceTileCommandHandler
{
    private readonly GameRepository gameRepository;

    public PlaceTileCommandHandler(GameRepository gameRepository)
    {
        this.gameRepository = gameRepository;
    }

    public async Task<OneOf<Success, GameNotFound, Unprivileged, InvalidTurnPhase, TileMustBeDrawnFirst, TileCannotBePlaced>> Handle(
        PlaceTileCommand command,
        CancellationToken cancellationToken
    )
    {
        var game = await gameRepository.GetById(command.GameId, cancellationToken);

        if (game is null)
        {
            return new GameNotFound(command.GameId);
        }

        if (game.CurrentPlayer.UserId != command.UserId)
        {
            return new Unprivileged();
        }

        var result = game.PlaceTile(command.Q, command.R, command.Rotation);

        return result.Match<OneOf<Success, GameNotFound, Unprivileged, InvalidTurnPhase, TileMustBeDrawnFirst, TileCannotBePlaced>>(
            success => success,
            invalidTurnPhase => new InvalidTurnPhase(game.Id),
            tileMustBeDrawnFirst => new TileMustBeDrawnFirst(game.Id),
            tileCannotBePlaced => new TileCannotBePlaced(game.Id)
        );
    }
}