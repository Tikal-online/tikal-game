using Games.Application.DataAccess;
using Games.Application.Mappers;
using Games.Contracts.Models;
using Games.Contracts.Queries;
using Shared.Application.Contexts;

namespace Games.Application.UseCases.GetGameForAuthenticatedPlayer;

public sealed class GetGameForAuthenticatedPlayerQueryHandler
{
    private readonly GameQueryContext gameQueryContext;

    private readonly AccountContext accountContext;

    public GetGameForAuthenticatedPlayerQueryHandler(
        GameQueryContext gameQueryContext,
        AccountContext accountContext
    )
    {
        this.gameQueryContext = gameQueryContext;
        this.accountContext = accountContext;
    }

    public async Task<GameModel?> Handle(
        GetGameForAuthenticatedPlayerQuery request,
        CancellationToken cancellationToken
    )
    {
        var game = await gameQueryContext.GetByUserId(accountContext.Account.UserId);

        if (game is null)
        {
            return null;
        }

        var gameModel = GameMapper.GameToGameModel(game);

        return gameModel;
    }
}