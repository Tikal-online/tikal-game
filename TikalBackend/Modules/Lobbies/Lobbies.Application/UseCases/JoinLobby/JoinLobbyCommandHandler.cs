using Lobbies.Application.DataAccess;
using Lobbies.Contracts.Commands;
using Lobbies.Contracts.Errors;
using Lobbies.Domain.Entities;
using Lobbies.Domain.Errors;
using OneOf;
using OneOf.Types;

namespace Lobbies.Application.UseCases.JoinLobby;

public sealed class JoinLobbyCommandHandler
{
    private readonly LobbyRepository lobbyRepository;

    private readonly PlayerQueryContext playerQueryContext;

    public JoinLobbyCommandHandler(
        LobbyRepository lobbyRepository,
        PlayerQueryContext playerQueryContext
    )
    {
        this.lobbyRepository = lobbyRepository;
        this.playerQueryContext = playerQueryContext;
    }

    public async Task<OneOf<Success, PlayerAlreadyInALobby, LobbyNotFound, LobbyFull, LobbyInGame>> Handle(
        JoinLobbyCommand request,
        CancellationToken cancellationToken
    )
    {
        var playerAlreadyInALobby = await playerQueryContext.PlayerExists(request.UserId, cancellationToken);

        if (playerAlreadyInALobby)
        {
            return new PlayerAlreadyInALobby();
        }

        var lobby = await lobbyRepository.GetById(request.LobbyId, cancellationToken);

        if (lobby is null)
        {
            return new LobbyNotFound(request.LobbyId);
        }

        var result = lobby.AddPlayer(request.Username, request.UserId);

        return result.Match<OneOf<Success, PlayerAlreadyInALobby, LobbyNotFound, LobbyFull, LobbyInGame>>(
            success => success,
            lobbyInGame => new LobbyInGame(lobby.Id),
            lobbyFull => new LobbyFull(lobby.Id)
        );
    }
}