using Lobbies.Application.DataAccess;
using Lobbies.Contracts.Queries;
using Shared.Application.Contexts;
using Shared.Contracts.Messaging;

namespace Lobbies.Application.UseCases.GetLobbyIdForAuthenticatedPlayer;

internal sealed class GetLobbyIdForAuthenticatedPlayerQueryHandler
    : QueryHandler<GetLobbyIdForAuthenticatedPlayerQuery, long?>
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
        return lobbyQueryContext.GetIdByUserIdAsync(accountContext.Account.UserId);
    }
}