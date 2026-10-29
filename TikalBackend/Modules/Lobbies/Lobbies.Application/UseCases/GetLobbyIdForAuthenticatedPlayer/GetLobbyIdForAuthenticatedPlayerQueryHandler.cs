using Lobbies.Application.DataAccess;
using Lobbies.Contracts.Queries;
using Shared.Application.Contexts;

namespace Lobbies.Application.UseCases.GetLobbyIdForAuthenticatedPlayer;

public sealed class GetLobbyIdForAuthenticatedPlayerQueryHandler
{
    private readonly LobbyQueryContext lobbyQueryContext;

    private readonly AccountContext accountContext;

    public GetLobbyIdForAuthenticatedPlayerQueryHandler(
        LobbyQueryContext lobbyQueryContext,
        AccountContext accountContext
    )
    {
        this.lobbyQueryContext = lobbyQueryContext;
        this.accountContext = accountContext;
    }

    public Task<long?> Handle(GetLobbyIdForAuthenticatedPlayerQuery request, CancellationToken cancellationToken)
    {
        return lobbyQueryContext.GetIdByUserId(accountContext.Account.UserId);
    }
}