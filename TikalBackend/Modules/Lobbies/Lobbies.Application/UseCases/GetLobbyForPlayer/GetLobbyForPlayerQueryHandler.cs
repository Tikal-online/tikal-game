using Lobbies.Application.DataAccess;
using Lobbies.Application.Mappers;
using Lobbies.Contracts.Models;
using Lobbies.Contracts.Queries;

namespace Lobbies.Application.UseCases.GetLobbyForPlayer;

public sealed class GetLobbyForPlayerQueryHandler
{
    private readonly LobbyQueryContext lobbyQueryContext;

    public GetLobbyForPlayerQueryHandler(LobbyQueryContext lobbyQueryContext)
    {
        this.lobbyQueryContext = lobbyQueryContext;
    }

    public async Task<LobbyModel?> Handle(
        GetLobbyForPlayerQuery request,
        CancellationToken cancellationToken
    )
    {
        var lobby = await lobbyQueryContext.GetByUserId(request.UserId);

        if (lobby is null)
        {
            return null;
        }

        var lobbyModel = LobbyMapper.LobbyToLobbyModel(lobby);

        return lobbyModel;
    }
}