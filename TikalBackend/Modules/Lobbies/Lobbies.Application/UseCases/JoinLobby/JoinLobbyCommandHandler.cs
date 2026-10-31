using Lobbies.Application.DataAccess;
using Lobbies.Contracts.Commands;
using Lobbies.Contracts.Errors;
using Lobbies.Domain.Entities;
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
        var playerAlreadyInALobby = await playerQueryContext.PlayerExists(request.UserId);

        if (playerAlreadyInALobby)
        {
            return new PlayerAlreadyInALobby();
        }

        var lobby = await lobbyRepository.GetById(request.LobbyId);

        if (lobby is null)
        {
            return new LobbyNotFound(request.LobbyId);
        }

        if (lobby.InGame)
        {
            return new LobbyInGame(lobby.Id);
        }

        if (lobby.IsFull)
        {
            return new LobbyFull(lobby.Id);
        }

        var colour = lobby.GetUnusedColour();

        var player = new Player
        {
            UserId = request.UserId,
            Name = request.Username,
            SelectedColour = colour,
            IsOwner = false,
            IsReady = false
        };

        lobby.AddPlayer(player);

        return new Success();
    }
}