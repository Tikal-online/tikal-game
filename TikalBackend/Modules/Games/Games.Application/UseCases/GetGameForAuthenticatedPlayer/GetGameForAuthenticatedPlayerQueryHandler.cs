using Accounts.Contracts.Queries;
using Games.Application.DataAccess;
using Games.Application.Mappers;
using Games.Contracts.Models;
using Games.Contracts.Queries;
using MediatR;
using Shared.Application.Contexts;
using Shared.Contracts.Messaging;

namespace Games.Application.UseCases.GetGameForAuthenticatedPlayer;

internal sealed class GetGameForAuthenticatedPlayerQueryHandler
    : QueryHandler<GetGameForAuthenticatedPlayerQuery, GameModel?>
{
    private readonly GameQueryContext gameQueryContext;

    private readonly ISender sender;

    private readonly AccountContext accountContext;

    public GetGameForAuthenticatedPlayerQueryHandler(
        GameQueryContext gameQueryContext,
        ISender sender,
        AccountContext accountContext
    )
    {
        this.gameQueryContext = gameQueryContext;
        this.sender = sender;
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

        var userIds = game.Players.Select(p => p.UserId).ToHashSet();

        var accounts = await sender.Send(new GetAccountsQuery(userIds), cancellationToken);

        var playerModels = PlayerMapper.PlayersToGamePlayerModels(game.Players, accounts);

        var gameModel = GameMapper.GameToGameModel(game);

        gameModel.Players = playerModels;

        return gameModel;
    }
}