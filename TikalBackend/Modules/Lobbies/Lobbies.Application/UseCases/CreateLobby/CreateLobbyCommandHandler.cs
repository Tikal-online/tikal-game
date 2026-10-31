using Lobbies.Application.DataAccess;
using Lobbies.Contracts.Commands;
using Lobbies.Contracts.Errors;
using Lobbies.Domain.Entities;
using OneOf;
using OneOf.Types;
using Shared.Domain.Enums;

namespace Lobbies.Application.UseCases.CreateLobby;

public sealed class CreateLobbyCommandHandler
{
    private readonly LobbyRepository lobbyRepository;

    private readonly PlayerQueryContext playerQueryContext;

    public CreateLobbyCommandHandler(
        LobbyRepository lobbyRepository,
        PlayerQueryContext playerQueryContext
    )
    {
        this.lobbyRepository = lobbyRepository;
        this.playerQueryContext = playerQueryContext;
    }

    public async Task<OneOf<Success, PlayerAlreadyInALobby>> Handle(
        CreateLobbyCommand request,
        CancellationToken cancellationToken
    )
    {
        var playerAlreadyInALobby = await playerQueryContext.PlayerExists(request.UserId);

        if (playerAlreadyInALobby)
        {
            return new PlayerAlreadyInALobby();
        }

        var player = new Player
        {
            UserId = request.UserId,
            Name = request.Username,
            SelectedColour = Colour.Red,
            IsOwner = true,
            IsReady = false
        };

        var lobby = new Lobby
        {
            Name = request.Name,
            MaxPlayers = request.MaxPlayers,
            Players = [player]
        };

        lobbyRepository.Create(lobby);

        return new Success();
    }
}