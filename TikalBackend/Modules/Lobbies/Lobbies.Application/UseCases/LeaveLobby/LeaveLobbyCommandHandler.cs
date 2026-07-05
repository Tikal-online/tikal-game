using Lobbies.Application.DataAccess;
using Lobbies.Contracts.Commands;
using Lobbies.Contracts.Errors;
using OneOf;
using OneOf.Types;
using Shared.Application.Contexts;
using Shared.Contracts.Messaging;

namespace Lobbies.Application.UseCases.LeaveLobby;

internal sealed class LeaveLobbyCommandHandler : CommandHandler<LeaveLobbyCommand, OneOf<Success, PlayerNotInALobby>>
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

    public async Task<OneOf<Success, PlayerNotInALobby>> Handle(
        LeaveLobbyCommand request,
        CancellationToken cancellationToken
    )
    {
        var player = await playerRepository.GetByUserIdAsync(accountContext.Account.UserId);

        if (player is null)
        {
            return new PlayerNotInALobby();
        }

        player.LeaveLobby();
        playerRepository.Delete(player);

        if (player.Lobby.IsEmpty)
        {
            lobbyRepository.Delete(player.Lobby);
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new Success();
    }
}