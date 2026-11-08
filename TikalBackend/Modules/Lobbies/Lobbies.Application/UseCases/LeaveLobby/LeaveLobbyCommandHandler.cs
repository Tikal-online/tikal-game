using Lobbies.Application.DataAccess;
using Lobbies.Contracts.Commands;
using Lobbies.Contracts.Errors;
using OneOf;
using OneOf.Types;

namespace Lobbies.Application.UseCases.LeaveLobby;

public sealed class LeaveLobbyCommandHandler
{
    private readonly LobbyRepository lobbyRepository;

    public LeaveLobbyCommandHandler(LobbyRepository lobbyRepository)
    {
        this.lobbyRepository = lobbyRepository;
    }

    public async Task<OneOf<Success, LobbyNotFound, PlayerNotInGivenLobby, LobbyInGame>> Handle(
        LeaveLobbyCommand request,
        CancellationToken cancellationToken
    )
    {
        var lobby = await lobbyRepository.GetById(request.LobbyId, cancellationToken);

        if (lobby is null)
        {
            return new LobbyNotFound(request.LobbyId);
        }

        var result = lobby.RemovePlayer(request.UserId);

        return result.Match<OneOf<Success, LobbyNotFound, PlayerNotInGivenLobby, LobbyInGame>>(
            success =>
            {
                if (lobby.IsEmpty)
                {
                    lobbyRepository.Delete(lobby);
                }

                return success;
            },
            playerNotInLobby => new PlayerNotInGivenLobby(lobby.Id),
            lobbyInGame => new LobbyInGame(lobby.Id)
        );
    }
}