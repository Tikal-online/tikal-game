using Games.Application.DataAccess;
using Games.Application.Mappers;
using Games.Contracts.Models;
using Games.Contracts.Queries;

namespace Games.Application.UseCases.GetGameForPlayer;

public sealed class GetGameForPlayerQueryHandler
{
    private readonly GameQueryContext gameQueryContext;

    public GetGameForPlayerQueryHandler(GameQueryContext gameQueryContext)
    {
        this.gameQueryContext = gameQueryContext;
    }

    public async Task<GameModel?> Handle(
        GetGameForPlayerQuery request,
        CancellationToken cancellationToken
    )
    {
        var game = await gameQueryContext.GetByUserId(request.UserId);

        if (game is null)
        {
            return null;
        }

        var gameModel = GameMapper.GameToGameModel(game);

        return gameModel;
    }
}