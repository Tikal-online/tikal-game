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

    public async Task<OneOf<TileModel, GameNotFound, NoTilesLeft>> Handle(
        GetNextTileQuery query,
        CancellationToken cancellationToken
    )
    {
        var game = await gameRepository.GetById(query.GameId, cancellationToken);

        if (game is null)
        {
            return new GameNotFound(query.GameId);
        }

        var tile = game.DrawTile();

        if (tile is null)
        {
            return new NoTilesLeft(game.Id);
        }

        var tileModel = GameMapper.TileToTileModel(tile);

        return tileModel;
    }
}