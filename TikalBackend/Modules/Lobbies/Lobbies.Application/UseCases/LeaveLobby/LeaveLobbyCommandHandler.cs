using Lobbies.Application.DataAccess;
using Lobbies.Contracts.Commands;
using Lobbies.Contracts.Errors;
using OneOf;
using OneOf.Types;
using Shared.Application.Contexts;
using Shared.Contracts.Messaging;

namespace Lobbies.Application.UseCases.LeaveLobby;

internal sealed class LeaveLobbyCommandHandler
    : CommandHandler<LeaveLobbyCommand, OneOf<Success, LobbyNotFound, PlayerNotInGivenLobby>>
{
    private readonly PlayerRepository playerRepository;

    private readonly LobbyRepository lobbyRepository;

    private readonly UnitOfWork unitOfWork;

    private readonly AccountContext accountContext;

    public LeaveLobbyCommandHandler(
        PlayerRepository playerRepository,
        LobbyRepository lobbyRepository,
        UnitOfWork unitOfWork,
        AccountContext accountContext
    )
    {
        this.playerRepository = playerRepository;
        this.lobbyRepository = lobbyRepository;
        this.unitOfWork = unitOfWork;
        this.accountContext = accountContext;
    }

    public async Task<OneOf<Success, LobbyNotFound, PlayerNotInGivenLobby>> Handle(
        LeaveLobbyCommand request,
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

        lobby.RemovePlayer(player);
        playerRepository.Delete(player);

        if (player.Lobby.IsEmpty)
        {
            lobbyRepository.Delete(player.Lobby);
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new Success();
    }
}