using Lobbies.Application.DataAccess;
using Lobbies.Application.Mappers;
using Lobbies.Contracts.Models;
using Lobbies.Contracts.Queries;
using Shared.Application.Contexts;

namespace Lobbies.Application.UseCases.GetLobbyForAuthenticatedPlayer;

public sealed class GetLobbyForAuthenticatedPlayerQueryHandler
{
    private readonly LobbyQueryContext lobbyQueryContext;

    private readonly AccountContext accountContext;

    public GetLobbyForAuthenticatedPlayerQueryHandler(
        LobbyQueryContext lobbyQueryContext,
        AccountContext accountContext
    )
    {
        this.lobbyQueryContext = lobbyQueryContext;
        this.accountContext = accountContext;
    }

    public async Task<LobbyModel?> Handle(
        GetLobbyForAuthenticatedPlayerQuery request,
        CancellationToken cancellationToken
    )
    {
        var lobby = await lobbyQueryContext.GetByUserId(accountContext.Account.UserId);

        if (lobby is null)
        {
            return null;
        }

        var lobbyModel = LobbyMapper.LobbyToLobbyModel(lobby);

        return lobbyModel;
    }
}