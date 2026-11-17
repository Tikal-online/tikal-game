using Games.Application.DataAccess;
using Games.Application.Mappers;
using Games.Contracts.Errors;
using Games.Contracts.Models;
using Games.Contracts.Queries;
using OneOf;

namespace Games.Application.UseCases.GetNextTile;

public sealed class GetNextTileQueryHandler
{
    private readonly GameRepository gameRepository;

    public GetNextTileQueryHandler(GameRepository gameRepository)
    {
        this.gameRepository = gameRepository;
    }

    public async Task<OneOf<TileModel, GameNotFound, InvalidTurnPhase, NoTilesLeft>> Handle(
        GetNextTileQuery query,
        CancellationToken cancellationToken
    )
    {
        var game = await gameRepository.GetById(query.GameId, cancellationToken);

        if (game is null)
        {
            return new GameNotFound(query.GameId);
        }

        var result = game.DrawTile();

        return result.Match<OneOf<TileModel, GameNotFound, InvalidTurnPhase, NoTilesLeft>>(
            tile => GameMapper.TileToTileModel(tile),
            invalidTurnPhase => new InvalidTurnPhase(game.Id),
            noTileLeft => new NoTilesLeft(game.Id)
        );
    }
}