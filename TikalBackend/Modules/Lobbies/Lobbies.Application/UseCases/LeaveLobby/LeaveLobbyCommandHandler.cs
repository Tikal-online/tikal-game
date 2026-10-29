using Lobbies.Application.DataAccess;
using Lobbies.Contracts.Commands;
using Lobbies.Contracts.Errors;
using OneOf;
using OneOf.Types;

namespace Lobbies.Application.UseCases.LeaveLobby;

public sealed class LeaveLobbyCommandHandler
{
    private readonly PlayerRepository playerRepository;

    private readonly LobbyRepository lobbyRepository;

    public LeaveLobbyCommandHandler(
        PlayerRepository playerRepository,
        LobbyRepository lobbyRepository
    )
    {
        this.playerRepository = playerRepository;
        this.lobbyRepository = lobbyRepository;
    }

    public async Task<OneOf<Success, LobbyNotFound, PlayerNotInGivenLobby, LobbyInGame>> Handle(
        LeaveLobbyCommand request,
        CancellationToken cancellationToken
    )
    {
        var lobby = await lobbyRepository.GetById(request.LobbyId);

        if (lobby is null)
        {
            return new LobbyNotFound(request.LobbyId);
        }

        var player = lobby.GetPlayer(request.UserId);

        if (player is null)
        {
            return new PlayerNotInGivenLobby(request.LobbyId);
        }

        if (lobby.InGame)
        {
            return new LobbyInGame(lobby.Id);
        }

        lobby.RemovePlayer(player);
        playerRepository.Delete(player);

        if (player.Lobby.IsEmpty)
        {
            lobbyRepository.Delete(player.Lobby);
        }

        return new Success();
    }
}