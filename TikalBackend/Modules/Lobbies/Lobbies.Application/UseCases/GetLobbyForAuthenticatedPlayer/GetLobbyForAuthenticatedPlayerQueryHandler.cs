using Accounts.Contracts.Queries;
using Lobbies.Application.DataAccess;
using Lobbies.Application.Mappers;
using Lobbies.Contracts.Models;
using Lobbies.Contracts.Queries;
using MediatR;
using Shared.Application.Contexts;
using Shared.Contracts.Messaging;

namespace Lobbies.Application.UseCases.GetLobbyForAuthenticatedPlayer;

internal sealed class GetLobbyForAuthenticatedPlayerQueryHandler
    : QueryHandler<GetLobbyForAuthenticatedPlayerQuery, LobbyModel?>
{
    private readonly LobbyQueryContext lobbyQueryContext;

    private readonly ISender sender;

    private readonly AccountContext accountContext;

    public GetLobbyForAuthenticatedPlayerQueryHandler(
        LobbyQueryContext lobbyQueryContext,
        ISender sender,
        AccountContext accountContext
    )
    {
        this.lobbyQueryContext = lobbyQueryContext;
        this.sender = sender;
        this.accountContext = accountContext;
    }

    public async Task<LobbyModel?> Handle(
        GetLobbyForAuthenticatedPlayerQuery request,
        CancellationToken cancellationToken
    )
    {
        var lobby = await lobbyQueryContext.GetByUserIdAsync(accountContext.Account.UserId);

        if (lobby is null)
        {
            return null;
        }

        var userIds = lobby.Players.Select(p => p.UserId).ToHashSet();

        var accounts = await sender.Send(new GetAccountsQuery(userIds), cancellationToken);

        var playerModels = PlayerMapper.PlayersToLobbyPlayerModels(lobby.Players, accounts);

        var lobbyModel = LobbyMapper.LobbyToLobbyModel(lobby);

        lobbyModel.Players = playerModels;

        return lobbyModel;
    }
}