using Lobbies.Application.DataAccess;
using Lobbies.Contracts.Queries;

namespace Lobbies.Application.UseCases.GetLobbyIdForPlayer;

public sealed class GetLobbyIdForPlayerQueryHandler
{
    private readonly LobbyQueryContext lobbyQueryContext;

    public GetLobbyIdForPlayerQueryHandler(LobbyQueryContext lobbyQueryContext)
    {
        this.lobbyQueryContext = lobbyQueryContext;
    }

    public Task<long?> Handle(GetLobbyIdForPlayerQuery request, CancellationToken cancellationToken)
    {
        return lobbyQueryContext.GetIdByUserId(request.UserId);
    }
}