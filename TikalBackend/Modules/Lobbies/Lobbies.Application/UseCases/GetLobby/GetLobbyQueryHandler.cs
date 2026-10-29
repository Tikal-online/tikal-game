using Lobbies.Application.DataAccess;
using Lobbies.Application.Mappers;
using Lobbies.Contracts.Models;
using Lobbies.Contracts.Queries;

namespace Lobbies.Application.UseCases.GetLobby;

public sealed class GetLobbyQueryHandler
{
    private readonly LobbyQueryContext lobbyQueryContext;

    public GetLobbyQueryHandler(LobbyQueryContext lobbyQueryContext)
    {
        this.lobbyQueryContext = lobbyQueryContext;
    }

    public async Task<LobbyModel?> Handle(GetLobbyQuery request, CancellationToken cancellationToken)
    {
        var lobby = await lobbyQueryContext.GetById(request.Id);

        if (lobby is null)
        {
            return null;
        }

        var lobbyModel = LobbyMapper.LobbyToLobbyModel(lobby);

        return lobbyModel;
    }
}