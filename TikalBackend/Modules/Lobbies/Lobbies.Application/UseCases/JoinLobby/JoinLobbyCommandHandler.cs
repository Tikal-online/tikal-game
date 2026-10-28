using Lobbies.Application.DataAccess;
using Lobbies.Contracts.Commands;
using Lobbies.Contracts.Errors;
using Lobbies.Domain.Entities;
using OneOf;
using OneOf.Types;
using Shared.Application.Contexts;

namespace Lobbies.Application.UseCases.JoinLobby;

public sealed class JoinLobbyCommandHandler
{
    private readonly LobbyRepository lobbyRepository;

    private readonly PlayerQueryContext playerQueryContext;

    private readonly AccountContext accountContext;

    public JoinLobbyCommandHandler(
        LobbyRepository lobbyRepository,
        PlayerQueryContext playerQueryContext,
        AccountContext accountContext
    )
    {
        this.lobbyRepository = lobbyRepository;
        this.playerQueryContext = playerQueryContext;
        this.accountContext = accountContext;
    }

    public async Task<OneOf<Success, PlayerAlreadyInALobby, LobbyNotFound, LobbyFull, LobbyInGame>> Handle(
        JoinLobbyCommand request,
        CancellationToken cancellationToken
    )
    {
        var playerAlreadyInALobby = await playerQueryContext.PlayerExists(accountContext.Account.UserId);

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
            UserId = accountContext.Account.UserId,
            Name = accountContext.Account.Name,
            SelectedColour = colour,
            IsOwner = false,
            IsReady = false
        };

        lobby.AddPlayer(player);

        return new Success();
    }
}