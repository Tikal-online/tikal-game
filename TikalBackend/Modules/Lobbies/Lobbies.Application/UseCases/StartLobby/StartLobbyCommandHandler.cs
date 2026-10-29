using Lobbies.Application.DataAccess;
using Lobbies.Contracts.Commands;
using Lobbies.Contracts.Errors;
using OneOf;
using OneOf.Types;
using Shared.Application.Contexts;
using Shared.Contracts.Errors;

namespace Lobbies.Application.UseCases.StartLobby;

public sealed class StartLobbyCommandHandler
{
    private readonly LobbyRepository lobbyRepository;

    private readonly AccountContext accountContext;

    public StartLobbyCommandHandler(
        LobbyRepository lobbyRepository,
        AccountContext accountContext
    )
    {
        this.lobbyRepository = lobbyRepository;
        this.accountContext = accountContext;
    }

    public async Task<OneOf<Success, LobbyNotFound, PlayerNotInGivenLobby, Unprivileged, LobbyCannotBeStarted>> Handle(
        StartLobbyCommand request,
        CancellationToken cancellationToken
    )
    {
        var lobby = await lobbyRepository.GetById(request.LobbyId);

        if (lobby is null)
        {
            return new LobbyNotFound(request.LobbyId);
        }

        var player = lobby.GetPlayer(accountContext.Account.UserId);

        if (player is null)
        {
            return new PlayerNotInGivenLobby(request.LobbyId);
        }

        if (!player.IsOwner)
        {
            return new Unprivileged();
        }

        if (!lobby.CanBeStarted)
        {
            return new LobbyCannotBeStarted(request.LobbyId);
        }

        lobby.Start();

        return new Success();
    }
}